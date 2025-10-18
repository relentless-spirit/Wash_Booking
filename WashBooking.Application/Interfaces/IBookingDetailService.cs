using System.Security.Claims;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Request;
using WashBooking.Application.DTOs.ServiceDTO.BookingDetailDTO.Response;
using WashBooking.Domain.Common;

namespace WashBooking.Application.Interfaces;

// <summary>
/// Service chịu trách nhiệm cho các thao tác chi tiết trên từng công việc (BookingDetail).
/// </summary>
public interface IBookingDetailService
{
    /// <summary>
    /// Cập nhật trạng thái của một công việc và tự động ghi log tiến trình.
    /// </summary>
    Task<Result> UpdateBookingDetailStatusAsync(Guid bookingId, Guid detailId, UpdateBookingDetailStatusRequest updateBookingDetailStatusRequest, ClaimsPrincipal user);

    /// <summary>
    /// Lấy lịch sử tiến trình của một công việc.
    /// </summary>
    Task<Result<BookingDetailProgressResponse>> GetBookingDetailProgressAsync(Guid bookingId, Guid detailId, ClaimsPrincipal user);

    /// <summary>
    /// Gán hoặc thay đổi nhân viên thực hiện cho một công việc.
    /// </summary>
    Task<Result> AssignStaffToBookingDetailAsync(Guid bookingId, Guid detailId, AssignStaffRequest request, ClaimsPrincipal user);

    /// <summary>
    /// Đánh dấu một công việc đã bắt đầu.
    /// </summary>
    Task<Result> StartServiceAsync(Guid bookingId, Guid detailId, ClaimsPrincipal user);

    /// <summary>
    /// Đánh dấu một công việc đã hoàn thành.
    /// </summary>
    Task<Result> CompleteServiceAsync(Guid bookingId, Guid detailId, CompleteServiceRequest request, ClaimsPrincipal user);
    
    /// <summary>
    /// Lấy danh sách công việc được giao
    /// </summary>
    Task<Result<IEnumerable<MyTaskResponse>>> GetMyTasksAsync(ClaimsPrincipal user);
}
