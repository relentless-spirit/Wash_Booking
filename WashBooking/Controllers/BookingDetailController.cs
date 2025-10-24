using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;
using WashBooking.Application.Interfaces;
using WashBooking.Domain.Enums;

namespace WashBooking.Controllers
{
    /// <summary>
    /// Controller chịu trách nhiệm cho các thao tác trên từng công việc chi tiết (BookingDetail).
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/bookings/{bookingId}/details")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Staff)}")] // Chỉ Admin và Staff mới được truy cập
    public class BookingDetailController : ControllerBase
    {
        private readonly IBookingDetailService _bookingDetailService;

        public BookingDetailController(IBookingDetailService bookingDetailService)
        {
            _bookingDetailService = bookingDetailService;
        }

        /// <summary>
        /// Cập nhật trạng thái của một công việc (ví dụ: chuyển sang QualityCheck).
        /// </summary>
        [HttpPut("{detailId:guid}/status")]
        public async Task<IActionResult> UpdateBookingDetailStatus(Guid bookingId, Guid detailId, [FromBody] UpdateBookingDetailStatusRequest request)
        {
            var result = await _bookingDetailService.UpdateBookingDetailStatusAsync(bookingId, detailId, request, User);

            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Validation")) return UnprocessableEntity(result.Errors);
                if (result.Error.Code.Contains("NotFound")) return NotFound(result.Error);
                // if (result.Error.Code.Contains("PermissionDenied") || result.Error.Code.Contains("Unassigned")) return StatusCode(StatusCodes.Status403Forbidden, result.Error);
                if (result.Error.Code.Contains("InvalidAction") || result.Error.Code.Contains("BookingNotReady") || result.Error.Code.Contains("InvalidTransition")) 
                    return Conflict(result.Error);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        [AllowAnonymous]
        /// <summary>
        /// Lấy lịch sử tiến trình của một công việc.
        /// </summary>
        [HttpGet("{detailId:guid}/progress")]
        public async Task<IActionResult> GetBookingDetailProgress(Guid bookingId, Guid detailId)
        {
            var result = await _bookingDetailService.GetBookingDetailProgressAsync(bookingId, detailId, User);

            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("NotFound")) return NotFound(result.Error);
                if (result.Error.Code.Contains("Forbidden")) return StatusCode(StatusCodes.Status403Forbidden, result.Error);
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gán hoặc thay đổi nhân viên thực hiện cho một công việc (Chỉ Admin).
        /// </summary>
        [HttpPut("{detailId:guid}/assign")]
        [Authorize(Roles = nameof(Role.Admin))] // Ghi đè, chỉ cho phép Admin
        public async Task<IActionResult> AssignStaffToBookingDetail(Guid bookingId, Guid detailId, [FromBody] AssignStaffRequest request)
        {
            var result = await _bookingDetailService.AssignStaffToBookingDetailAsync(bookingId, detailId, request, User);

            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Validation")) return UnprocessableEntity(result.Errors);
                if (result.Error.Code.Contains("NotFound") || result.Error.Code.Contains("StaffNotFound")) return NotFound(result.Error);
                if (result.Error.Code.Contains("Forbidden") || result.Error.Code.Contains("PermissionDenied")) return StatusCode(StatusCodes.Status403Forbidden, result.Error);
                if (result.Error.Code.Contains("Conflict")) return Conflict(result.Error);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Đánh dấu một công việc đã bắt đầu.
        /// </summary>
        [HttpPut("{detailId:guid}/start")]
        public async Task<IActionResult> StartService(Guid bookingId, Guid detailId)
        {
            var result = await _bookingDetailService.StartServiceAsync(bookingId, detailId, User);

            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("NotFound")) return NotFound(result.Error);
                if (result.Error.Code.Contains("PermissionDenied")) return StatusCode(StatusCodes.Status403Forbidden, result.Error);
                if (result.Error.Code.Contains("InvalidBookingStatus") ||
                result.Error.Code.Contains("InvalidTransition") ||
                result.Error.Code.Contains("Unassigned"))
                    return Conflict(result.Error);
                return BadRequest(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Đánh dấu một công việc đã hoàn thành.
        /// </summary>
        [HttpPut("{detailId:guid}/complete")]
        public async Task<IActionResult> CompleteService(Guid bookingId, Guid detailId, [FromBody] CompleteServiceRequest request)
        {
            var result = await _bookingDetailService.CompleteServiceAsync(bookingId, detailId, request, User);

            if (result.IsFailure)
            {
                if (result.Error.Code.Contains("Validation")) return UnprocessableEntity(result.Errors);
                if (result.Error.Code.Contains("NotFound")) return NotFound(result.Error);
                if (result.Error.Code.Contains("PermissionDenied")) return StatusCode(StatusCodes.Status403Forbidden, result.Error);
                if (result.Error.Code.Contains("InvalidTransition")) return Conflict(result.Error);
                return BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}