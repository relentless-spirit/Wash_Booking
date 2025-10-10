namespace WashBooking.Application.DTOs.BookingDTO.Response;

    /// <summary>
    /// DTO chứa thông tin chi tiết đầy đủ của một Booking dành cho nhân viên và admin.
    /// </summary>
public class StaffBookingDetailResponse
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

    public List<StaffBookingDetailItemDto> Details { get; set; } = new();
}

public class StaffBookingDetailItemDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string VehicleDescription { get; set; } = string.Empty;
    public string? AssigneeName { get; set; } // Tên nhân viên
    public Guid? AssigneeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
}