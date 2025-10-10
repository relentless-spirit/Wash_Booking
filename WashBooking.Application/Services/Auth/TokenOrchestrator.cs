using AutoMapper;
using Microsoft.Extensions.Options;
using WashBooking.Application.Common.Settings;
using WashBooking.Application.DTOs.UserProfileDTO;
using WashBooking.Application.Interfaces;
using WashBooking.Application.Interfaces.Auth;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services.Auth
{
    public class TokenOrchestrator : ITokenOrchestrator
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        public TokenOrchestrator(IJwtProvider jwtProvider, IUnitOfWork unitOfWork, IMapper mapper, IOptions<JwtSettings> jwtSettingsOptions)
        {
            _jwtProvider = jwtProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtSettings = jwtSettingsOptions.Value;
        }
        public async Task<TokenResult> GenerateAndSaveTokensAsync(Account account)
        {
            var userProfileDto = _mapper.Map<UserProfileDTO>(account.UserProfile);
            var tokenResult = _jwtProvider.GenerateTokens(userProfileDto);

            // Lưu Refresh Token vào cơ sở dữ liệu
            var refreshToken = new RefreshToken
            {
                Token = tokenResult.RefreshToken,
                AccountId = account.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            };
            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return tokenResult;
        }
    }
}
