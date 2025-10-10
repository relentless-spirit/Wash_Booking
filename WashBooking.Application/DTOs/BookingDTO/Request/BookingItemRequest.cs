namespace WashBooking.Application.DTOs.BookingDTO;

public class BookingItemRequest
{
    /// <summary>
    /// ID của BookingDetail đã tồn tại.
    /// Sẽ là Guid.Empty nếu đây là một item mới được thêm vào.
    /// </summary>
    public Guid? Id { get; set; }
    public Guid ServiceId { get; set; }
    public string VehicleDescription { get; set; } // Ví dụ: "Vario Đỏ - Biển số 59-T1 12345"
}