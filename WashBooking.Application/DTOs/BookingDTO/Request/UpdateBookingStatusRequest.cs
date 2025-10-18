using WashBooking.Domain.Enums;

namespace WashBooking.Application.DTOs.BookingDTO.Request;

public class UpdateBookingStatusRequest
{
    public BookingStatus NewStatus { get; set; }
}