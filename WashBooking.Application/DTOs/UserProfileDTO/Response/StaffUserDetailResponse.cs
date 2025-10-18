using WashBooking.Domain.Enums;

namespace WashBooking.Application.DTOs.UserProfileDTO.Response;

/// <summary>
/// DTO chứa thông tin liên lạc cần thiết của một khách hàng, 
/// được trả về cho nhân viên (Staff).
/// </summary>
public class StaffUserDetailResponse
{
        /// <summary>
        /// ID của UserProfile.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của khách hàng.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Địa chỉ email của khách hàng.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Số điện thoại của khách hàng để liên lạc khi cần.
        /// </summary>
        public string? Phone { get; set; }
}