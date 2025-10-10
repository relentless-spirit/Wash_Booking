namespace WashBooking.Application.DTOs.BookingDTO.Response;

/// <summary>
/// DTO chứa thông tin chi tiết đầy đủ của một Booking dành cho khách hàng đã đăng nhập.
/// </summary>
public class CustomerBookingDetailResponse
{
    public Guid Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<CustomerBookingDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// DTO chứa thông tin của một công việc chi tiết trong Booking.
/// </summary>
public class CustomerBookingDetailItemDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string VehicleDescription { get; set; } = string.Empty;
    public string? AssigneeName { get; set; } // Tên nhân viên (có thể ẩn đi nếu muốn)
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
}