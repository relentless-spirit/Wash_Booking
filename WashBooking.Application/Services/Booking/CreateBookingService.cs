using AutoMapper;
using Microsoft.Extensions.Configuration;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using FluentValidation;
using WashBooking.Application.Common.Settings;
using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Application.Interfaces.Booking;
using WashBooking.Domain.Enums;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services
{
    public class CreateBookingService : ICreateBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBookingRequest> _createBookingRequestValidator;
        private readonly int _maxCapacity = 3;
        private readonly int _bufferMinutes = 10;

        public CreateBookingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            /*IConfiguration configuration,*/
            IValidator<CreateBookingRequest> createBookingRequestValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createBookingRequestValidator = createBookingRequestValidator;
            /*_maxCapacity = configuration.GetValue<int>("BookingSettings:MaxConcurrentBookings", 3);
            _bufferMinutes = configuration.GetValue<int>("BookingSettings:TransitionBufferInMinutes", 10);*/

        }

        public async Task<Result<Guid>> CreateBookingForUserAsync(Guid userId, CreateBookingRequest request)
        {
            var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(userId);
            if (userProfile is null)
            {
                return Result<Guid>.Failure(new Error("Booking.Add.UserProfile.NotFound", "User does not exist."));
            }

            return await ProcessBookingCreationAsync(request, userProfile.FullName, userProfile.Phone,
                userProfile.Email, userId);
        }

        public async Task<Result<Guid>> CreateBookingForGuestAsync(CreateBookingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.GuestName) || string.IsNullOrWhiteSpace(request.GuestPhone) ||
                string.IsNullOrWhiteSpace(request.GuestEmail))
            {
                return Result<Guid>.Failure(new Error("Booking.Add.Validation",
                    "Guest name, phone number, and email are required."));
            }

            return await ProcessBookingCreationAsync(request, request.GuestName, request.GuestPhone, request.GuestEmail,
                null);
        }

        private async Task<Result<Guid>> ProcessBookingCreationAsync(
            CreateBookingRequest request,
            string customerName,
            string customerPhone,
            string? customerEmail,
            Guid? userProfileId)
        {
            try
            {
                // ===== STEP 1: VALIDATION AND PREPARATION =====
                var validationResult = await _createBookingRequestValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new Error("Booking.Add.Validation", e.ErrorMessage))
                        .ToList();
                    return Result<Guid>.Failure(errors);
                }

                var (plannedJobs, totalAmount) = await PreparePlannedJobsAsync(request.Items);
                if (plannedJobs is null)
                {
                    return Result<Guid>.Failure(new Error("Booking.Add.ServiceInvalid",
                        "One or more services are invalid."));
                }

                // ===== STEP 2: EXECUTE SCHEDULING ALGORITHM =====
                var scheduleResult = await ScheduleJobsAsync(request.BookingDateTime, plannedJobs);
                if (!scheduleResult.IsSuccess)
                {
                    return Result<Guid>.Failure(new Error("Booking.Add.SchedulingFailed", scheduleResult.ErrorMessage));
                }

                var bookingId = Guid.NewGuid();
                var bookingCode = BookingCodeGenerator.Generate(bookingId, request.BookingDateTime);
                
                // ===== STEP 3: CREATE ENTITIES =====
                var newBooking = new Booking
                {
                    Id = bookingId,
                    UserProfileId = userProfileId,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CustomerEmail = customerEmail ?? string.Empty,
                    BookingDatetime = DateTime.SpecifyKind(request.BookingDateTime, DateTimeKind.Utc),
                    TotalAmount = totalAmount,
                    Status = BookingStatus.Scheduled,
                    PaymentStatus = "Unpaid",
                    BookingCode = bookingCode,
                    Note = request.Note,
                };
                await _unitOfWork.BookingRepository.AddAsync(newBooking);

                foreach (var scheduledJob in scheduleResult.ScheduledJobs)
                {
                    var bookingDetail = new BookingDetail
                    {
                        Booking = newBooking,
                        ServiceId = scheduledJob.Service.Id,
                        VehicleDescription = scheduledJob.VehicleDescription,
                        AssigneeId = scheduledJob.AssigneeId,
                        Status = BookingStatus.Scheduled,
                        Price = scheduledJob.Service.Price,
                        DurationMinutes = scheduledJob.Service.DurationMinutes ?? 0,
                        PlannedStartTime = DateTime.SpecifyKind(scheduledJob.StartTime, DateTimeKind.Utc),
                        PlannedEndTime = DateTime.SpecifyKind(scheduledJob.EndTime, DateTimeKind.Utc),
                        ActualStartTime = null,
                        ActualEndTime = null
                    };
                    await _unitOfWork.BookingDetailRepository.AddAsync(bookingDetail);

                    var progress = new BookingDetailProgress
                    {
                        BookingDetail = bookingDetail,
                        Status = BookingStatus.Scheduled,
                        Note = "Appointment has been automatically created by the system."
                    };
                    await _unitOfWork.BookingDetailProgressRepository.AddAsync(progress);
                }

                // ===== STEP 4: SAVE TO DATABASE =====
                await _unitOfWork.SaveChangesAsync();

                // (Optional: Send confirmation email here)
                return Result<Guid>.Success(newBooking.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(new Error("Booking.Add.System.Error", ex.Message));
            }
        }

        // --- Helper methods and classes ---

        private async Task<(List<PlannedJob>? jobs, decimal totalAmount)> PreparePlannedJobsAsync(
            List<BookingItemRequest> items)
        {
            var plannedJobs = new List<PlannedJob>();
            decimal totalAmount = 0;
            foreach (var item in items)
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync(item.ServiceId);
                if (service is null || (bool)!service.IsActive) return (null, 0);

                totalAmount += service.Price;
                plannedJobs.Add(new PlannedJob
                {
                    Service = service,
                    VehicleDescription = item.VehicleDescription,
                    DurationWithBuffer = (service.DurationMinutes ?? 0) + _bufferMinutes
                });
            }

            return (plannedJobs, totalAmount);
        }

        private async Task<ScheduleResult> ScheduleJobsAsync(DateTime requestedStartTime, List<PlannedJob> jobs)
        {
            var utcRequestedStartTime = DateTime.SpecifyKind(requestedStartTime, DateTimeKind.Utc);

            var allStaff = await _unitOfWork.UserProfileRepository.GetByRoleAsync(nameof(Role.Staff));
            if (!allStaff.Any())
            {
                return new ScheduleResult
                    { IsSuccess = false, ErrorMessage = "No staff available to serve." };
            }

            var scheduledJobs = new List<ScheduledJob>();
            var staffTimelines = allStaff.ToDictionary(s => s.Id, s => new List<(DateTime Start, DateTime End)>());

            // Load existing schedules for the day to optimize
            var existingBookings =
                await _unitOfWork.BookingDetailRepository.GetScheduledBookingsForDateAsync(
                    DateOnly.FromDateTime(utcRequestedStartTime));
            foreach (var existing in existingBookings)
            {
                if (existing.AssigneeId.HasValue && staffTimelines.ContainsKey(existing.AssigneeId.Value))
                {
                    staffTimelines[existing.AssigneeId.Value]
                        .Add((existing.PlannedStartTime, existing.PlannedEndTime));
                }
            }

            foreach (var job in jobs.OrderByDescending(j => j.DurationWithBuffer))
            {
                var jobStartTime = utcRequestedStartTime;
                var jobEndTime = jobStartTime.AddMinutes(job.DurationWithBuffer);

                UserProfile? bestStaff = null;
                double minWorkload = double.MaxValue;

                // Find the best staff member (available and with least workload)
                foreach (var staff in allStaff)
                {
                    bool isAvailable = !staffTimelines[staff.Id]
                        .Any(slot => jobStartTime < slot.End && jobEndTime > slot.Start);

                    if (isAvailable)
                    {
                        double currentWorkload = staffTimelines[staff.Id]
                            .Sum(slot => (slot.End - slot.Start).TotalMinutes);

                        if (currentWorkload < minWorkload)
                        {
                            minWorkload = currentWorkload;
                            bestStaff = staff;
                        }
                    }
                }

                if (bestStaff is null)
                {
                    return new ScheduleResult
                    {
                        IsSuccess = false, ErrorMessage = "Unable to find a suitable schedule. The time slot is full."
                    };
                }

                // Assign job and update temporary timeline
                var scheduledJob = new ScheduledJob
                {
                    Service = job.Service,
                    VehicleDescription = job.VehicleDescription,
                    AssigneeId = bestStaff.Id,
                    StartTime = jobStartTime,
                    EndTime = jobEndTime
                };
                scheduledJobs.Add(scheduledJob);
                staffTimelines[bestStaff.Id].Add((jobStartTime, jobEndTime));
            }

            return new ScheduleResult { IsSuccess = true, ScheduledJobs = scheduledJobs };
        }

        private class PlannedJob
        {
            public Service Service { get; set; } = null!;
            public string VehicleDescription { get; set; } = string.Empty;
            public int DurationWithBuffer { get; set; }
        }

        private class ScheduledJob : PlannedJob
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public Guid AssigneeId { get; set; }
        }

        private class ScheduleResult
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
            public List<ScheduledJob> ScheduledJobs { get; set; } = new();
        }
    }
}

