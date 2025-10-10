using WashBooking.Domain.Enums;

namespace WashBooking.Application.DTOs.BookingDTO;

public class CreateBookingRequest
{
    // Dùng cho khách vãng lai
    public string? GuestName { get; set; } = null;
    public string? GuestPhone { get; set; } = null;
    public string? GuestEmail { get; set; } = null;
    
    // Thông tin chung
    public DateTime BookingDateTime { get; set; }
    public List<BookingItemRequest> Items { get; set; } = new();
    public string? Note { get; set; }
}