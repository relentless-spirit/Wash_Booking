using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using WashBooking.Application.Common.Settings.StateManagement;
using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Application.DTOs.BookingDTO.Request;
using WashBooking.Application.DTOs.BookingDTO.Response;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Application.Interfaces.Booking;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Enums;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateBookingRequest> _updateBookingRequestValidator;
    private readonly IValidator<GetPagedRequest> _getPagedRequestValidator;
    private readonly IValidator<UpdateBookingStatusRequest> _updateBookingStatusRequestValidator;
    
    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateBookingRequest> updateBookingRequestValidator, IValidator<GetPagedRequest> getPagedRequestValidator, IValidator<UpdateBookingStatusRequest> updateBookingStatusRequestValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _updateBookingRequestValidator = updateBookingRequestValidator;
        _getPagedRequestValidator = getPagedRequestValidator;
        _updateBookingStatusRequestValidator = updateBookingStatusRequestValidator;
    }
    
    public async Task<Result> UpdateBookingAsync(Guid id, UpdateBookingRequest request)
    {
        // 1️⃣ Lấy booking hiện tại
        var booking = await _unitOfWork.BookingRepository.GetAllInfoBookingByIdAsync(id);

        if (booking is null)
            return Result.Failure(new Error("Booking.Update.NotFound", $"Not found booking with id: {id}."));

        if (booking.Status == BookingStatus.Completed)
            return Result.Failure(new Error("Booking.Update.Completed", "Booking is completed. You can't update it."));

        // 2️⃣ Validate request
        var validationResult = _updateBookingRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Booking.Update.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }

        // 3️⃣ Map các field đơn giản (bỏ qua BookingDetails)
        _mapper.Map(request, booking);
        booking.UpdatedAt = DateTime.UtcNow;

        // 4️⃣ Chuẩn bị dữ liệu cũ và mới
        var existingDetails = booking.BookingDetails.ToList();
        var requestDetails = request.Items ?? new List<BookingItemRequest>();

        // 5️⃣ Thêm các detail mới
        foreach (var detailReq in requestDetails)
        {

            if (detailReq.Id is null)
            {
                // 🆕 Detail mới
                var newDetail = _mapper.Map<BookingDetail>(detailReq);
                newDetail.BookingId = booking.Id;
                newDetail.CreatedAt = DateTime.UtcNow;
                newDetail.Status = BookingStatus.Scheduled; // auto hệ thống set

                await _unitOfWork.BookingDetailRepository.AddAsync(newDetail);

                // ➕ Ghi log progress
                var progress = new BookingDetailProgress
                {
                    BookingDetail = newDetail,
                    Status = BookingStatus.Scheduled,
                    Note = "Booking detail has been automatically created by the system.",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.BookingDetailProgressRepository.AddAsync(progress);
            }
            else
            {
                var existingDetail = existingDetails.FirstOrDefault(d => d.Id == detailReq.Id);
                if (existingDetail != null)
                {
                    _mapper.Map(detailReq, existingDetail);
                    existingDetail.UpdatedAt = DateTime.UtcNow;
                }

            }
        }

        // 6️⃣ Xử lý các detail bị xóa khỏi request → hệ thống tự cập nhật sang Cancelled
        var removedDetails = existingDetails
            .Where(d => !requestDetails.Any(r => r.Id == d.Id))
            .ToList();

        foreach (var removed in removedDetails)
        {
            if (removed.Status != BookingStatus.Cancelled)
            {
                removed.Status = BookingStatus.Cancelled;
                removed.UpdatedAt = DateTime.UtcNow;

                var progress = new BookingDetailProgress
                {
                    BookingDetailId = removed.Id,
                    Status = BookingStatus.Cancelled,
                    Note = "Booking detail has been automatically cancelled by the system.",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.BookingDetailProgressRepository.AddAsync(progress);
            }
        }

        // 7️⃣ Lưu tất cả thay đổi
        try
        {
            // ✅ Thêm dòng này để đánh dấu entity Booking đã thay đổi
            _unitOfWork.BookingRepository.Update(booking);
            
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Booking.Update.Database.Error",
                $"Update booking failed. Please try again later. Error: {ex.Message}"));
        }

        return Result.Success();
    }

    public async Task<Result> UpdateStatusBookingAsync(Guid id, UpdateBookingStatusRequest updateBookingStatusRequest)
    {
        var validationResult = _updateBookingStatusRequestValidator.Validate(updateBookingStatusRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Booking.UpdateStatusBooking.Validation", e.ErrorMessage))
                .ToList();
            return Result.Failure(errors);
        }
        
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking is null)
            return Result.Failure(new Error("Booking.UpdateStatusBooking.NotFound", "Booking not found"));
        
        if (!BookingStateTransitions.CanTransitionTo(booking.Status, updateBookingStatusRequest.NewStatus))
        {
            return Result.Failure(new Error("Service.Complete.InvalidTransition", 
                $"Cannot complete service from the current status '{booking.Status}'."));
        }
        
        _mapper.Map(updateBookingStatusRequest, booking);

        try
        {
            await _unitOfWork.SaveChangesAsync();      
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Booking.UpdateStatusBooking.Database.Error", "Update failed. Please try again later."));      
        }
        
        return Result.Success();
    }

    public async Task<Result> DeleteBookingAsync(Guid id)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
        if (booking is null)
        {
            return Result.Failure(new Error("Booking.Delete.NotFound", "Booking not found"));       
        }

        booking.Status = BookingStatus.Cancelled;
        try
        {
            await _unitOfWork.SaveChangesAsync();       
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Booking.Delete.Database.Error", "Booking delete failed. Please try again later."));       
        }
        return Result.Success();
    }

    public async Task<Result<PagedResult<AdminBookingDetailResponse>>> GetBookingsAsync(GetPagedRequest getPagedRequest)
    {
        var validationResult = await _getPagedRequestValidator.ValidateAsync(getPagedRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Booking.GetPaged.Validation", e.ErrorMessage))
                .ToList();
            return Result<PagedResult<AdminBookingDetailResponse>>.Failure(errors);
        }

        Expression<Func<Booking, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(getPagedRequest.Search))
        {
            filter = b => 
                b.CustomerName.Contains(getPagedRequest.Search) || 
                b.CustomerPhone.Contains(getPagedRequest.Search) || 
                b.CustomerEmail.Contains(getPagedRequest.Search) || 
                b.BookingCode.Contains(getPagedRequest.Search);
        }

        var result =
            await _unitOfWork.BookingRepository.GetPagedAsync(getPagedRequest.PageIndex, getPagedRequest.PageSize,
                filter);
        
        var bookingDtos = _mapper.Map<PagedResult<AdminBookingDetailResponse>>(result);
        return Result<PagedResult<AdminBookingDetailResponse>>.Success(bookingDtos);
    }

    public async Task<Result<object>> GetBookingByIdAsync(Guid id, ClaimsPrincipal? user)
    {
        var booking = await _unitOfWork.BookingRepository.GetAllInfoBookingByIdAsync(id);
        if (booking is null)
        {
            return Result<object>.Failure(new Error("Booking.GetById.NotFound", "Booking not found"));
        }
        if (user is not null && user.Identity is { IsAuthenticated: true })
        {
            // === VÒNG KIỂM TRA 1: BẠN CÓ PHẢI LÀ ADMIN/STAFF KHÔNG? ===
            if (user.IsInRole("Admin") || user.IsInRole("Staff"))
            {
                // Nếu đúng -> Trả về "góc nhìn" chi tiết nhất
                var staffDto = _mapper.Map<StaffBookingDetailResponse>(booking);
                return Result<object>.Success(staffDto);
            }
        
            // === VÒNG KIỂM TRA 2: NẾU KHÔNG, BẠN CÓ PHẢI LÀ CHỦ SỞ HỮU KHÔNG? ===
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (Guid.TryParse(userIdString, out var userId) && booking.UserProfileId == userId)
            {
                // Nếu đúng -> Trả về "góc nhìn" của khách hàng
                var customerDto = _mapper.Map<CustomerBookingDetailResponse>(booking);
                return Result<object>.Success(customerDto);
            }
        }
    
        // === TRƯỜNG HỢP MẶC ĐỊNH ===
        // Nếu bạn là Guest, hoặc đã đăng nhập nhưng không phải Admin/Staff và cũng không phải chủ sở hữu
        return Result<object>.Failure(new Error("Auth.Forbidden", "Bạn không có quyền xem lịch hẹn này."));

    }

    public async Task<Result<object>> TrackByCodeAsync(string bookingCode, ClaimsPrincipal? user)
    {
        var booking = await _unitOfWork.BookingRepository.GetBookingByBookingCodeAsync(bookingCode);
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (booking is null)
        {
            return Result<object>.Failure(new Error("Booking.Track.NotFound", "Booking not found"));
        }

        if (user is not null && user.Identity.IsAuthenticated)
        {
            if (user.IsInRole("Admin"))
            {
                var adminDto = _mapper.Map<AdminBookingDetailResponse>(booking);
                return Result<object>.Success(adminDto);
            }
            else if (user.IsInRole("Staff"))
            {
                var staffDto = _mapper.Map<StaffBookingDetailResponse>(booking);
                return Result<object>.Success(staffDto);
            }
            else if (Guid.TryParse(userIdString, out var userId) && booking.UserProfileId == userId)
            {
                var customerDto = _mapper.Map<CustomerBookingDetailResponse>(booking);
                return Result<object>.Success(customerDto);
            }
        }
        
        var guestDto = _mapper.Map<GuestBookingStatusResponse>(booking);
        return Result<object>.Success(guestDto);
    }
    
    public async Task<Result<List<CustomerBookingDetailResponse>>> GetMyBooking(ClaimsPrincipal? user)
    {
        if (user is null || user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return Result<List<CustomerBookingDetailResponse>>.Failure(new Error("Booking.MyBooking.Forbidden", "User is not authenticated."));
        }

        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Result<List<CustomerBookingDetailResponse>>.Failure(new Error("Booking.MyBooking.InvalidUser", "Invalid user id."));
        }

        var bookings = await _unitOfWork.BookingRepository.GetAllBookingByUserIdAsync(userId);
        if (bookings == null || bookings.Count == 0)
        {
            return Result<List<CustomerBookingDetailResponse>>.Failure(new Error("Booking.MyBooking.NotFound", "Booking not found"));
        }
        
        var customerDto = _mapper.Map<List<CustomerBookingDetailResponse>>(bookings);
        return Result<List<CustomerBookingDetailResponse>>.Success(customerDto);
    }


}