namespace WashBooking.Application.DTOs.BookingDTO.Response;

/// <summary>
/// DTO chứa thông tin theo dõi tối thiểu, an toàn cho khách vãng lai.
/// Đại diện cho "trang chi tiết đơn hàng".
/// </summary>
public class GuestBookingStatusResponse
{
    public string BookingCode { get; set; } = string.Empty;
    public DateTime BookingDateTime { get; set; }

    /// <summary>
    /// Trạng thái TỔNG THỂ của toàn bộ booking.
    /// Ví dụ: "Đang tiến hành", "Đã hoàn thành".
    /// </summary>
    public string OverallStatus { get; set; } = string.Empty;

    /// <summary>
    /// Danh sách chi tiết của từng công việc trong booking.
    /// </summary>
    public List<BookingJobTimelineDto> Jobs { get; set; } = new();
}

/// <summary>
/// DTO chứa "dòng thời gian" của một công việc cụ thể.
/// </summary>
public class BookingJobTimelineDto
{
    public string VehicleDescription { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái HIỆN TẠI của riêng công việc này.
    /// Ví dụ: "Đang rửa", "Đang kiểm tra".
    /// </summary>
    public string CurrentJobStatus { get; set; } = string.Empty;
    
    /// <summary>
    /// Lịch sử chi tiết các bước đã trải qua của công việc này.
    /// </summary>
    public List<ProgressStepDto> Timeline { get; set; } = new();
}

/// <summary>
/// DTO đại diện cho một bước trong "dòng thời gian".
/// </summary>
public class ProgressStepDto
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime Timestamp { get; set; }
}