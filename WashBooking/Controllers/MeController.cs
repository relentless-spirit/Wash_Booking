using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WashBooking.Application.DTOs.UserProfileDTO.Request;
using WashBooking.Application.Interfaces;
using WashBooking.Application.Interfaces.Booking;

namespace WashBooking.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[ApiController]

public class MeController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBookingDetailService _bookingDetailService;
    private readonly IBookingService _bookingService;
    
    public MeController(IUserService userService, IBookingDetailService bookingDetailServiceService, IBookingService bookingService)
    {
        _userService = userService;
        _bookingDetailService = bookingDetailServiceService;
        _bookingService = bookingService;   
    }
    
    //     •  GET /api/user/profile - Get current user profile [Customer/Staff/Admin]
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var result = await _userService.GetUserProfileAsync(User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Value);   
    }
    
// •  PUT /api/user/profile - Update current user profile [Customer/Staff/Admin]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest updateUserProfileRequest)
    {
        var result = await _userService.UpdateAsync(updateUserProfileRequest, User);
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
    
    [HttpGet("tasks")]
    public async Task<IActionResult> GetMyAssignedTasks()
    {
        var result = await _bookingDetailService.GetMyTasksAsync(User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound")) return NotFound(result.Error);
            if (result.Error.Code.Contains("Forbidden")) return Unauthorized(result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _bookingService.GetMyBooking(User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound")) return NotFound(result.Error);
            if (result.Error.Code.Contains("Forbidden") || result.Error.Code.Contains("InvalidUser")) return Unauthorized(result.Error);
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }
}