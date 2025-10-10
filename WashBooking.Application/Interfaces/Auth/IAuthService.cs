using WashBooking.Application.DTOs.AuthDTO.LoginDTO;
using WashBooking.Application.DTOs.AuthDTO.LogoutDTO;
using WashBooking.Application.DTOs.AuthDTO.RefreshTokenDTO;
using WashBooking.Application.DTOs.AuthDTO.RegisterDTO;
using WashBooking.Domain.Common;

namespace WashBooking.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequest registerRequest);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest);
        Task<Result> LogoutAsync(LogoutRequest logoutRequest);
        Task<Result<TokenResult>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}
