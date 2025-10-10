using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Domain.Common;

namespace WashBooking.Application.Interfaces.Booking;

public interface ICreateBookingService
{
    /// <summary>
    /// Tạo một lịch hẹn mới cho người dùng đã đăng nhập.
    /// </summary>
    Task<Result<Guid>> CreateBookingForUserAsync(Guid userId, CreateBookingRequest createBookingRequest);

    /// <summary>
    /// Tạo một lịch hẹn mới cho khách vãng lai.
    /// </summary>
    Task<Result<Guid>> CreateBookingForGuestAsync(CreateBookingRequest createBookingRequest);
}