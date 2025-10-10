using WashBooking.Domain.Entities;

namespace WashBooking.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken?> IsTokenValid(string refreshToken);
        Task UpdateRefreshToken(RefreshToken refreshToken);
    }
}
