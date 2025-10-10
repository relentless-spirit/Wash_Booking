namespace WashBooking.Application.DTOs.AuthDTO.LoginDTO
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; } = null!;
        /// <summary>
        /// Thông tin cơ bản của người dùng để hiển thị trên giao diện.
        /// </summary>
        public WashBooking.Application.DTOs.UserProfileDTO.UserProfileDTO UserProfileDTO { get; set; } = null!;
    }
}
