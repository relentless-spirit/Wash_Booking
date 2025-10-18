using WashBooking.Domain.Enums;

namespace WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;

public class UpdateBookingDetailStatusRequest
{
    public BookingStatus NewStatus { get; set; }
    public string? Note { get; set; }
}