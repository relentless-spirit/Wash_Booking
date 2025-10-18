namespace WashBooking.Application.DTOs.ServiceDTO;

public class GetPagedRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}