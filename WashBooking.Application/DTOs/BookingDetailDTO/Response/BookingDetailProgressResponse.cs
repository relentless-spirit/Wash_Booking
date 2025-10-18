namespace WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response;

/// <summary>
/// DTO chứa lịch sử tiến trình của một công việc chi tiết.
/// </summary>
public class BookingDetailProgressResponse
{
    public Guid BookingDetailId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public List<ProgressStepDto> Timeline { get; set; } = new();
}