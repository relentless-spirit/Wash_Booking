using WashBooking.Domain.Enums;

namespace WashBooking.Application.DTOs.UserProfileDTO.Response;

/// <summary>
/// DTO chứa thông tin chi tiết của một người dùng trong danh sách trả về cho Admin.
/// </summary>
public class AdminUserDetailResponse
{
    /// <summary>
    /// ID của UserProfile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tên đăng nhập của tài khoản.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Họ và tên đầy đủ của người dùng.
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Địa chỉ email của người dùng.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Số điện thoại của người dùng.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Địa chỉ của người dùng.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Vai trò của người dùng trong hệ thống (ví dụ: "Admin", "Staff", "Customer").
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Loại tài khoản (ví dụ: "Local", "Google").
    /// </summary>
    public string AccountType { get; set; }
    
    /// <summary>
    /// Trạng thái hoạt động của hồ sơ người dùng.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Ngày tạo hồ sơ.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}