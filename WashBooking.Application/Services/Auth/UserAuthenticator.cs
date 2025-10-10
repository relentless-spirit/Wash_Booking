using WashBooking.Application.Interfaces.Auth;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services.Auth
{
    public class UserAuthenticator : IUserAuthenticator
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserAuthenticator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Account?> AuthenticateAsync(string usernameOrEmail, string password)
        {
            var account = await _unitOfWork.AccountRepository.FindByUsernameOrEmailAsync(usernameOrEmail);
            if (account is null) return null;
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
            return isPasswordValid ? account : null;
        }
    }
}
