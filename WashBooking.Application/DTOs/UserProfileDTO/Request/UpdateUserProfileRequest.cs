namespace WashBooking.Application.DTOs.UserProfileDTO.Request;

public class UpdateUserProfileRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}