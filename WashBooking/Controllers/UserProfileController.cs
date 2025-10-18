using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.DTOs.UserProfileDTO.Request;
using WashBooking.Application.Interfaces;

namespace WashBooking.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserProfileController(IUserService userService)
    {
        _userService = userService;
    }
    
// •  GET /api/user - Get paginated users list [Admin/Staff]
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]   
    public async Task<IActionResult> GetPaginatedUsers([FromQuery] GetPagedRequest getPagedRequest)
    {
        var result = await _userService.GetPaginatedUsersAsync(getPagedRequest, User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }   
        return Ok(result.Value);   
    }
// •  GET /api/user/{id} - Get user profile by ID [Admin/Staff] -- chưa check
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id, User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return Ok(result.Value);  
    }
// •  PUT /api/user/{id} - Update user profile by ID [Admin]
    [HttpPut("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserProfileByIdForAdmin(Guid id,
        [FromBody] UpdateUserProfileRequest updateUserProfileRequest)
    {
        var result = await _userService.UpdateUserProfileByIdForAdmin(id, updateUserProfileRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return NoContent();
    }
// •  PUT /api/user/{id}/activate - Activate/Deactivate user [Admin]
    [HttpPut("admin/activate/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateUser(Guid id, [FromBody] ActivateUserRequest activateUserRequest)
    {
        var result = await _userService.ActivateUserAsync(id, activateUserRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return NoContent();   
    }

// •  PUT /api/user/{id}/role - Change user role [Admin]
    [HttpPut("admin/role/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeUserRole(Guid id, UpdateUserRoleRequest updateUserRoleRequest)
    {
        var result = await _userService.ChangeUserRoleAsync(id, updateUserRoleRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return NoContent();  
    }
}