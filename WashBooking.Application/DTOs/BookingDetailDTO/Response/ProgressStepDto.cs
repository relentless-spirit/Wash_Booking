namespace WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response;

public class ProgressStepDto
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime Timestamp { get; set; }
}