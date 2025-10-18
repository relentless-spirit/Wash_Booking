using System.Security.Claims;
using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Application.DTOs.BookingDTO.Request;
using WashBooking.Application.DTOs.BookingDTO.Response;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Domain.Common;

namespace WashBooking.Application.Interfaces.Booking;

public interface IBookingService
{
    Task<Result> UpdateBookingAsync(Guid id, UpdateBookingRequest updateBookingRequest);
    Task<Result> UpdateStatusBookingAsync(Guid id, UpdateBookingStatusRequest updateBookingStatusRequest);
    Task<Result> DeleteBookingAsync(Guid id);
    Task<Result<PagedResult<AdminBookingDetailResponse>>> GetBookingsAsync(GetPagedRequest getPagedRequest);
    Task<Result<object>> GetBookingByIdAsync(Guid id, ClaimsPrincipal? user);
    Task<Result<object>> TrackByCodeAsync(string bookingCode, ClaimsPrincipal? user);
}