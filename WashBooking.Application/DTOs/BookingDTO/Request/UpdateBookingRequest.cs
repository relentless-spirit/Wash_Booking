namespace WashBooking.Application.DTOs.BookingDTO;

/// <summary>
/// DTO chứa các thông tin có thể được cập nhật cho một booking.
/// </summary>
public class UpdateBookingRequest
{
    /// <summary>
    /// Ngày và giờ hẹn mới.
    /// </summary>
    public DateTime BookingDateTime { get; set; }

    /// <summary>
    /// Danh sách CÔNG VIỆC MỚI mà khách hàng muốn.
    /// Backend sẽ so sánh với danh sách cũ để xác định thay đổi.
    /// </summary>
    public List<BookingItemRequest> Items { get; set; } = new();
        
    public string? Note { get; set; }
}