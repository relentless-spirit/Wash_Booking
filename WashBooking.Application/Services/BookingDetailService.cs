using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using WashBooking.Application.Common.Settings.StateManagement;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response;
using WashBooking.Application.Interfaces;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Enums;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services;

public class BookingDetailService : IBookingDetailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateBookingDetailStatusRequest> _updateBookingDetailStatusRequestValidator;
    private readonly IValidator<AssignStaffRequest> _assignStaffRequestValidator;
    private readonly IValidator<CompleteServiceRequest> _completeServiceRequestValidator;
    
    public BookingDetailService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateBookingDetailStatusRequest> updateBookingDetailStatusRequestValidator, IValidator<AssignStaffRequest> assignStaffRequestValidator, IValidator<CompleteServiceRequest> completeServiceRequestValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _updateBookingDetailStatusRequestValidator = updateBookingDetailStatusRequestValidator;   
        _assignStaffRequestValidator = assignStaffRequestValidator;  
        _completeServiceRequestValidator = completeServiceRequestValidator;   
    }
    
    public async Task<Result> UpdateBookingDetailStatusAsync(Guid bookingId, Guid detailId,
        UpdateBookingDetailStatusRequest request, ClaimsPrincipal user)
    {
        // === BƯỚC 1: VALIDATION ===
        var validationResult = _updateBookingDetailStatusRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("BookingDetail.UpdateStatus.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }
    
        // === BƯỚC 2: LẤY DỮ LIỆU ===
        var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailByIdAsync(detailId); 
        if (bookingDetail is null || bookingDetail.BookingId != bookingId)
        {
            return Result.Failure(new Error("BookingDetail.UpdateStatus.NotFound", "Booking detail not found."));
        }

        // === BƯỚC 3: KIỂM TRA NGHIỆP VỤ & QUYỀN HẠN ===

        // 1. Chặn dùng hàm này cho các hành động đặc biệt
        if (request.NewStatus == BookingStatus.ServiceInProgress || request.NewStatus == BookingStatus.Completed)
        {
            return Result.Failure(new Error("BookingDetail.UpdateStatus.InvalidAction", 
                "Please use the specific 'Start Service' or 'Complete Service' action to change to this status."));
        }
    
        // 2. Kiểm tra trạng thái của Booking cha (ĐÚNG & QUAN TRỌNG)
        if (bookingDetail.Booking.Status == BookingStatus.Scheduled)
        {
            return Result.Failure(new Error("BookingDetail.UpdateStatus.BookingNotReady",
                "Cannot update service status while the main booking has not been checked in."));
        }

        // 3. Kiểm tra State Machine
        if (!BookingStateTransitions.CanTransitionTo(bookingDetail.Status, request.NewStatus))
        {
            return Result.Failure(new Error("BookingDetail.UpdateStatus.InvalidTransition",
                $"Cannot transition from status '{bookingDetail.Status}' to '{request.NewStatus}'."));
        }

        // 4. Kiểm tra quyền hạn của người thực hiện
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier).Value;
        Guid.TryParse(userIdString, out var userId);
        var isUserAdmin = user.IsInRole(Role.Admin.ToString());
    
        if (!isUserAdmin)
        {
            if (bookingDetail.AssigneeId == null)
            {
                return Result.Failure(new Error("BookingDetail.UpdateStatus.Unassigned", 
                    "This service is unassigned and can only be managed by an Administrator."));
            }
            if (userId != bookingDetail.AssigneeId)
            {
                return Result.Failure(new Error("BookingDetail.UpdateStatus.PermissionDenied", 
                    "Permission denied. Only the assigned staff or an Administrator can update the service."));
            }
        }
        
        // === BƯỚC 4: CẬP NHẬT VÀ GHI LOG ===
        _mapper.Map(request, bookingDetail);
        bookingDetail.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.BookingDetailRepository.Update(bookingDetail); // Thêm dòng này để rõ ràng

        var progressLog = new BookingDetailProgress
        {
            BookingDetailId = detailId,
            Status = request.NewStatus,
            Note = request.Note, 
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.BookingDetailProgressRepository.AddAsync(progressLog);
    
        // === BƯỚC 5: LƯU DATABASE ===
        try
        {
            await _unitOfWork.SaveChangesAsync();      
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("BookingDetail.UpdateStatus.Database.Error", "Update failed. Please try again later."));      
        }
    
        return Result.Success();
    }

    public async Task<Result<BookingDetailProgressResponse>> GetBookingDetailProgressAsync(Guid bookingId, Guid detailId, ClaimsPrincipal user)
    {
        var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailByIdAsync(detailId);
        if (bookingDetail is null)
        {
            return Result<BookingDetailProgressResponse>.Failure(new Error("BookingDetail.GetBookingDetailProgress.NotFound", "Booking detail not found"));
        }

        if (bookingDetail.BookingId != bookingId)
        {
            return Result<BookingDetailProgressResponse>.Failure(new Error("BookingDetail.GetBookingDetailProgress.Forbidden", "You are not allowed to view this booking detail"));
        }
        
        var bookingDetailDto = _mapper.Map<BookingDetailProgressResponse>(bookingDetail);
        return Result<BookingDetailProgressResponse>.Success(bookingDetailDto);
    }

    public async Task<Result> AssignStaffToBookingDetailAsync(Guid bookingId, Guid detailId, AssignStaffRequest request, ClaimsPrincipal user)
    {
        var validationResult = _assignStaffRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("BookingDetail.AssignStaffToBookingDetail.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }

        if (!user.IsInRole(Role.Admin.ToString()))
        {
            return Result.Failure(new Error("BookingDetail.AssignStaffToBookingDetail.PermissionDenied", 
                "Permission denied. Only an Administrator can assign a staff to a service."));
        }
        
        var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailByIdAsync(detailId);
        if (bookingDetail is null || bookingDetail.BookingId != bookingId)
        {
            return Result.Failure(new Error("BookingDetail.AssignStaff.NotFound", "The specified service detail was not found in this booking."));
        }
        
        var staffToAssign = await _unitOfWork.UserProfileRepository.GetByIdAsync(request.NewAssigneeId);
        if (staffToAssign is null || staffToAssign.Role != Role.Staff)
        {
            return Result.Failure(new Error("BookingDetail.AssignStaff.StaffNotFound", "The selected staff member is invalid or not found."));
        }


        
        if (bookingDetail.Status == BookingStatus.Completed || bookingDetail.Status == BookingStatus.Cancelled || bookingDetail.Status == BookingStatus.QualityCheck || bookingDetail.Status == BookingStatus.ReadyForPickup)
        {
            return Result.Failure(new Error("BookingDetail.AssignStaff.Conflict", $"Cannot assign staff because the service is already '{bookingDetail.Status}'."));
        }
        
        _mapper.Map(request, bookingDetail);

        try
        {
            await _unitOfWork.SaveChangesAsync();     
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("BookingDetail.AssignStaff.Database.Error", "Assign staff failed. Please try again later."));      
        }
        
        return Result.Success();    
    }

    public async Task<Result> StartServiceAsync(Guid bookingId, Guid detailId, ClaimsPrincipal user)
    {
        var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailByIdAsync(detailId);
        if (bookingDetail is null || bookingDetail.BookingId != bookingId)
        {
            return Result.Failure(new Error("BookingDetail.StartService.NotFound", "The specified service detail was not found in this booking."));
        }

        if (bookingDetail.Booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
        {
            return Result.Failure(new Error("Booking.StartService.InvalidBookingStatus",
                $"Cannot start a service because booking is {bookingDetail.Booking.Status}."));
        }
        
        if (!BookingStateTransitions.CanTransitionTo(bookingDetail.Status, BookingStatus.ServiceInProgress))
        {
            return Result.Failure(new Error("Service.Start.InvalidTransition", 
                $"Cannot start service from the current status '{bookingDetail.Status}'."));
        }
        
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Result.Failure(new Error("Service.Start.InvalidUser", "Cannot determine current user ID."));
        }
        
        if (bookingDetail.AssigneeId == null)
        {
            return Result.Failure(new Error("Service.Start.Unassigned", "This service is unassigned and cannot be started."));
        }
        
        if (userId != bookingDetail.AssigneeId)
        {
            return Result.Failure(new Error("Service.Start.PermissionDenied", "Only the assigned staff can start this service."));
        }
        
        //chang status & time start
        var currentTime = DateTime.UtcNow;
        bookingDetail.Status = BookingStatus.ServiceInProgress;
        bookingDetail.UpdatedAt = currentTime;
        bookingDetail.ActualStartTime = currentTime;
        
        var progressLog = new BookingDetailProgress
        {
            BookingDetailId = detailId,
            Status = bookingDetail.Status,
            Note = "Service has been started.", 
            CreatedByUserId = userId,
            CreatedAt = currentTime
        };
        
        await _unitOfWork.BookingDetailProgressRepository.AddAsync(progressLog);

        try
        {
            await  _unitOfWork.SaveChangesAsync();     
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Service.Start.Database.Error", "Start service failed. Please try again later."));      
        }
        
        return Result.Success();   
    }

    public async Task<Result> CompleteServiceAsync(Guid bookingId, Guid detailId, CompleteServiceRequest request, ClaimsPrincipal user)
    {
        var validationResult = _completeServiceRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Service.Complete.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);   
        }
        // === BƯỚC 1: LẤY DỮ LIỆU VÀ KIỂM TRA TỒN TẠI ===
        var bookingDetail = await _unitOfWork.BookingDetailRepository.GetBookingDetailByIdAsync(detailId);
        if (bookingDetail is null || bookingDetail.BookingId != bookingId)
        {
            return Result.Failure(new Error("Service.Complete.NotFound", "The specified service detail was not found in this booking."));
        }

        // === BƯỚC 2: KIỂM TRA QUYỀN HẠN & ĐIỀU KIỆN NGHIỆP VỤ ===

        // 1. Lấy thông tin người dùng một cách an toàn
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Result.Failure(new Error("Service.Complete.InvalidUser", "Cannot determine current user ID."));
        }
        
        // 2. Kiểm tra xem người thực hiện có phải là người được gán không
        // (Cho phép cả Admin thực hiện để linh hoạt)
        if (!user.IsInRole(Role.Admin.ToString()) && userId != bookingDetail.AssigneeId)
        {
            return Result.Failure(new Error("Service.Complete.PermissionDenied", "Only the assigned staff or an Administrator can complete this service."));
        }

        // 3. Kiểm tra State Machine
        if (!BookingStateTransitions.CanTransitionTo(bookingDetail.Status, BookingStatus.Completed))
        {
            return Result.Failure(new Error("Service.Complete.InvalidTransition", 
                $"Cannot complete service from the current status '{bookingDetail.Status}'."));
        }
        
        var currentTime = DateTime.UtcNow;

        // Cập nhật trạng thái & thời gian kết thúc
        bookingDetail.Status = BookingStatus.Completed;
        bookingDetail.ActualEndTime = currentTime;
        bookingDetail.UpdatedAt = currentTime;
        _unitOfWork.BookingDetailRepository.Update(bookingDetail);
        
        var progressLog = new BookingDetailProgress
        {
            BookingDetailId = detailId,
            Status = BookingStatus.Completed,
            Note = request.Note ?? "Service has been completed.",
            CreatedByUserId = userId,
            CreatedAt = currentTime
        };
        await _unitOfWork.BookingDetailProgressRepository.AddAsync(progressLog);

        // === BƯỚC 4: LƯU VÀO DATABASE ===
        try
        {
            await _unitOfWork.SaveChangesAsync();     
        }
        catch (Exception e)
        {
            // Ghi log lỗi (e) ở đây
            return Result.Failure(new Error("Service.Complete.Database.Error", "Failed to complete service. Please try again later."));      
        }
        
        return Result.Success();   
    }

    public async Task<Result<IEnumerable<MyTaskResponse>>> GetMyTasksAsync(ClaimsPrincipal user)
    {
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Result<IEnumerable<MyTaskResponse>>.Failure(new Error("GetMyTask.GetMyTasksAsync.InvalidUser", "Cannot determine current user ID."));
        }

        var bookingDetails = await _unitOfWork.BookingDetailRepository.GetAssignedDetailsByUserIdAsync(userId);
        if (bookingDetails is null)
        {
            return Result<IEnumerable<MyTaskResponse>>.Failure(new Error("GetMyTask.GetMyTasksAsync.NotFound", "No tasks found for the current user."));
        }
        
        var myTasks = _mapper.Map<IEnumerable<MyTaskResponse>>(bookingDetails);
        return Result<IEnumerable<MyTaskResponse>>.Success(myTasks);
    }
}