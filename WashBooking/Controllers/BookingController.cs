using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WashBooking.Application.DTOs.BookingDTO;
using WashBooking.Application.DTOs.BookingDTO.Request;
using WashBooking.Application.DTOs.ServiceDTO;
using WashBooking.Application.Interfaces.Booking;
using WashBooking.Common;
using WashBooking.Domain.Common;

namespace WashBooking.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly ICreateBookingService _createBookingService;
    private readonly IBookingService _bookingService;
    
    public BookingController(ICreateBookingService createBookingService, IBookingService bookingService)
    {
        _createBookingService = createBookingService;
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] GetPagedRequest getPagedRequest)
    {
        var result = await _bookingService.GetBookingsAsync(getPagedRequest);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }   
        return Ok(result.Value);  
    }
    
    [HttpGet("{bookingCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackBooking(string bookingCode)
    {
        var result = await _bookingService.TrackByCodeAsync(bookingCode, User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }   
        return Ok(result.Value);   
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBookingById(Guid id)
    {
        
        var result = await _bookingService.GetBookingByIdAsync(id, User);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }   
        return Ok(result.Value); 
    }
    
    [HttpPost("customer")]
    public async Task<IActionResult> AddBookingByCustomer([FromBody] CreateBookingRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            // Trả về lỗi nếu token không chứa User ID hợp lệ.
            // Lỗi này rất hiếm khi xảy ra nếu token được tạo đúng cách.
            return Unauthorized(new ErrorResponse("Auth.InvalidToken", "Invalid token or missing user information."));
        }
        var id = Guid.Parse(userIdString);
        var result = await _createBookingService.CreateBookingForUserAsync(userId, request);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }  
        return CreatedAtAction(nameof(GetBookingById), new { id = result.Value }, new { BookingId = result.Value });
    }

    [HttpPost("guest")]
    [AllowAnonymous]
    public async Task<IActionResult> AddBookingByGuest([FromBody] CreateBookingRequest request)
    {
        var result = await _createBookingService.CreateBookingForGuestAsync(request);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
            {
                return UnprocessableEntity(result.Errors);
            }
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }  
        return CreatedAtAction(nameof(GetBookingById), new { id = result.Value }, new { BookingId = result.Value });
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] UpdateBookingRequest request)
    {
        var result = await _bookingService.UpdateBookingAsync(id, request);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            if (result.Error.Code.Contains("Validation"))
                return UnprocessableEntity(result.Errors);
            return BadRequest(result.Error);
        }
        return NoContent();
    }

    [Authorize(Roles = "Admin, Staff")]
    [HttpPut("status/{id:guid}")]
    public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] UpdateBookingStatusRequest request)
    {
        var result = await _bookingService.UpdateStatusBookingAsync(id, request);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            if (result.Error.Code.Contains("Validation"))
                return UnprocessableEntity(result.Errors);
            return BadRequest(result.Error);
        }
        return NoContent(); 
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBooking(Guid id)
    {
        var result = await _bookingService.DeleteBookingAsync(id);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("NotFound"))
                return NotFound(result.Error);
            return BadRequest(result.Error);
        }
        return NoContent();  
    }
}