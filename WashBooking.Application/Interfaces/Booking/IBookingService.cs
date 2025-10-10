using System.Security.Claims;
using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Application.DTOs.BookingDTO.Response;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Domain.Common;

namespace WashBooking.Application.Interfaces.Booking;

public interface IBookingService
{
    Task<Result> UpdateBookingAsync(Guid id, UpdateBookingRequest updateBookingRequest);
    Task<Result> DeleteBookingAsync(Guid id);
    Task<Result<List<AdminBookingDetailResponse>>> GetBookingsAsync();
    Task<Result<object>> GetBookingByIdAsync(Guid id, ClaimsPrincipal? user);
    Task<Result<object>> TrackByCodeAsync(string bookingCode, ClaimsPrincipal? user);
}