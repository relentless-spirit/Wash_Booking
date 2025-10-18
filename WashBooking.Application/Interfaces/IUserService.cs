using System.Security.Claims;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.ServiceDTO.Response;
using WashBooking.Application.DTOs.UserProfileDTO.Request;
using WashBooking.Application.DTOs.UserProfileDTO.Response;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;

namespace WashBooking.Application.Interfaces;

public interface IUserService
{
    // •  GET /api/user/profile/me- Get current user profile [Customer/Staff/Admin]
    Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal user);
    // •  PUT /api/user/profile - Update current user profile [Customer/Staff/Admin]
    Task<Result> UpdateAsync(UpdateUserProfileRequest updateUserProfileRequest, ClaimsPrincipal user);
    // •  GET /api/user - Get paginated users list [Admin/Staff]
    Task<Result<PagedResult<object>>> GetPaginatedUsersAsync(GetPagedRequest getPagedRequest, ClaimsPrincipal user);
    // •  GET /api/user/{id} - Get user profile by ID [Admin/Staff]
    Task<Result<object>> GetUserByIdAsync(Guid id, ClaimsPrincipal user);
    // •  PUT /api/user/{id} - Update user profile by ID [Admin]
    Task<Result> UpdateUserProfileByIdForAdmin(Guid id, UpdateUserProfileRequest updateUserProfileRequest);
    // •  PUT /api/user/{id}/activate - Activate/Deactivate user [Admin]
    Task<Result> ActivateUserAsync(Guid id, ActivateUserRequest activateUserRequest);
    // •  PUT /api/user/{id}/role - Change user role [Admin]
    Task<Result> ChangeUserRoleAsync(Guid id, UpdateUserRoleRequest updateUserRoleRequest);
}