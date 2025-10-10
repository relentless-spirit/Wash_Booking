namespace WashBooking.Application.DTOs.BookingDTO.Response;

/// <summary>
/// DTO chứa thông tin CHI TIẾT của một Booking dành cho Admin.
/// </summary>
public class AdminBookingDetailResponse
{
    public Guid Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    // Một danh sách các công việc chi tiết
    public List<AdminBookingDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// DTO chứa thông tin của một công việc chi tiết trong Booking.
/// </summary>
public class AdminBookingDetailItemDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string VehicleDescription { get; set; } = string.Empty;
    public string? AssigneeName { get; set; } // Tên nhân viên
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
}