using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WashBooking.Application.DTOs.UserProfileDTO;
using WashBooking.Application.Interfaces;

namespace WashBooking.Infrastructure.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResult GenerateTokens(UserProfileDTO userProfileDTO)
        {
            // ===== 1. TẠO ACCESS TOKEN (DÙNG THƯ VIỆN MỚI) =====
            var handler = new JsonWebTokenHandler();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userProfileDTO.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, userProfileDTO.Email),
                new("fullName", userProfileDTO.FullName),
                new(ClaimTypes.Role, userProfileDTO.Role.ToString())
            };

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationInMinutes = int.Parse(_configuration["JwtSettings:TokenExpirationInMinutes"] ?? "10");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };

            var accessTokenString = handler.CreateToken(descriptor);

            // ===== 2. TẠO REFRESH TOKEN (Không thay đổi) =====
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshTokenString = Convert.ToBase64String(randomNumber);

            // ===== 3. TRẢ VỀ KẾT QUẢ =====
            return new TokenResult(
                AccessToken: accessTokenString,
                AccessTokenExpiration: descriptor.Expires.GetValueOrDefault(),
                RefreshToken: refreshTokenString
            );
        }
    }
}
