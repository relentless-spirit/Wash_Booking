using WashBooking.Domain.Enums;

namespace WashBooking.Application.DTOs.UserProfileDTO.Request;

public class UpdateUserRoleRequest
{
    public Role Role { get; set; }
}