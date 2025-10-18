namespace WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response;

/// <summary>
/// DTO chứa thông tin tóm tắt về một công việc được giao cho nhân viên.
/// </summary>
public class MyTaskResponse
{
    public Guid BookingId { get; set; }
    public Guid DetailId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string VehicleDescription { get; set; } = string.Empty;
    public DateTime PlannedStartTime { get; set; }
    public DateTime PlannedEndTime { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
}