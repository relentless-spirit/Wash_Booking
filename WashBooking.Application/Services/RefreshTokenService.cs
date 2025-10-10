using WashBooking.Application.Interfaces;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RefreshTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<RefreshToken?> IsTokenValid(string refreshToken)
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(refreshToken);
            if (token is null) return null;
            if (token.IsRevoked || token.ExpiresAt < DateTime.UtcNow) return null;

            return token;
        }

        public async Task UpdateRefreshToken(RefreshToken refreshToken)
        {
            _unitOfWork.RefreshTokenRepository.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
