namespace WashBooking.Application.DTOs.ServiceDTO;

public class CreateServiceRequest
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? DurationMinutes { get; set; }
}