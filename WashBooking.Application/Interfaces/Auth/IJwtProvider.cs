using WashBooking.Application.DTOs.UserProfileDTO;

namespace WashBooking.Application.Interfaces
{
    /// <summary>
    /// Hợp đồng cho việc tạo JSON Web Tokens.
    /// </summary>
    public interface IJwtProvider
    {
        /// <summary>
        /// Tạo ra một cặp Access Token và Refresh Token cho người dùng.
        /// </summary>
        /// <param name="userProfile">Thông tin người dùng để đưa vào token.</param>
        /// <returns>Một đối tượng chứa cả Access Token và Refresh Token.</returns>
        TokenResult GenerateTokens(UserProfileDTO userProfileDTO);
    }

    /// <summary>
    /// Đối tượng chứa kết quả của việc tạo token.
    /// </summary>
    public record TokenResult(
        string AccessToken,
        DateTime AccessTokenExpiration,
        string RefreshToken);
}
