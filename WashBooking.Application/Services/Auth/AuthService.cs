using AutoMapper;
using FluentValidation;
using WashBooking.Application.DTOs.AuthDTO.LoginDTO;
using WashBooking.Application.DTOs.AuthDTO.LogoutDTO;
using WashBooking.Application.DTOs.AuthDTO.RefreshTokenDTO;
using WashBooking.Application.DTOs.AuthDTO.RegisterDTO;
using WashBooking.Application.DTOs.UserProfileDTO;
using WashBooking.Application.Interfaces;
using WashBooking.Application.Interfaces.Auth;
using WashBooking.Domain.Common;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Enums;
using WashBooking.Domain.Interfaces.Persistence;

namespace WashBooking.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<LogoutRequest> _logoutValidator;
        private readonly IValidator<RefreshTokenRequest> _refreshTokenValidator;
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly ITokenOrchestrator _tokenOrchestrator;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUnitOfWork unitOfWork, IMapper mapper,IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator, IUserAuthenticator userAuthenticator, ITokenOrchestrator tokenOrchestrator, IValidator<LogoutRequest> logoutValidator, IValidator<RefreshTokenRequest> refreshTokenValidator, IRefreshTokenService refreshTokenService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _logoutValidator = logoutValidator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenService = refreshTokenService;
            _userAuthenticator = userAuthenticator;
            _tokenOrchestrator = tokenOrchestrator;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            var validationResult = _loginValidator.Validate(loginRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Auth.Login.Validation", e.ErrorMessage))
                    .ToList();
                return Result<LoginResponse>.Failure(errors);
            }

            var account = await _userAuthenticator.AuthenticateAsync(loginRequest.UsernameOrEmail, loginRequest.Password);

            if (account is null)
            {
                return Result<LoginResponse>.Failure(new Error("Auth.Login.InvalidCredentials", "Incorrect username or password."));
            }

            var userProfileDto = _mapper.Map<UserProfileDTO>(account.UserProfile);

            var tokenResult = await _tokenOrchestrator.GenerateAndSaveTokensAsync(account);

            var loginResponse = new LoginResponse
            {
                UserProfileDTO = userProfileDto,
                AccessToken = tokenResult.AccessToken,
                AccessTokenExpiration = tokenResult.AccessTokenExpiration,  
                RefreshToken = tokenResult.RefreshToken
            };

            return Result<LoginResponse>.Success(loginResponse);
        }

        public async Task<Result> LogoutAsync(LogoutRequest logoutRequest)
        {
            var validationResult = await _logoutValidator.ValidateAsync(logoutRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Auth.Logout.Validation", e.ErrorMessage))
                    .ToList();
                return Result.Failure(errors);
            }

            var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(logoutRequest.RefreshToken);
            if (refreshToken is null)
            {
                return Result.Failure(new Error("Auth.Logout.NotFound", "Refresh token is not found."));
            }

            refreshToken.IsRevoked = true;
            await _refreshTokenService.UpdateRefreshToken(refreshToken);
            return Result.Success();
        }

        public async Task<Result<TokenResult>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var validationResult = await _refreshTokenValidator.ValidateAsync(refreshTokenRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Auth.RefreshToken.Validation", e.ErrorMessage))
                    .ToList();
                return Result<TokenResult>.Failure(errors);
            }

            var savedToken = await _refreshTokenService.IsTokenValid(refreshTokenRequest.RefreshToken);
            if (savedToken is null)
            {
                return Result<TokenResult>.Failure(new Error("Auth.RefreshToken.Invalid", "Refresh token is invalid or expired."));
            }

            savedToken.IsRevoked = true;
            await _refreshTokenService.UpdateRefreshToken(savedToken);

            var account = await _unitOfWork.AccountRepository.GetByIdAsync(savedToken.AccountId);

            var newToken = await _tokenOrchestrator.GenerateAndSaveTokensAsync(account);
            return Result<TokenResult>.Success(newToken);
        }

        public async Task<Result> RegisterAsync(RegisterRequest registerRequest)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerRequest);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Auth.Register.Validation", e.ErrorMessage))
                    .ToList();
                return Result.Failure(errors);
            }
            var accountExisting = await _unitOfWork.AccountRepository.GetByUsernameAsync(registerRequest.UserName);
            if ( accountExisting != null)
            {
                return Result.Failure(new Error("Auth.Register.Username.Duplicate", "Username is existed"));
            }

            var emailExisting = await _unitOfWork.UserProfileRepository.GetByEmaiAsync(registerRequest.Email);
            if (emailExisting != null)
            {
                return Result.Failure(new Error("Auth.Register.Email.Duplicate", "Email is existed"));
            }

            var phoneExisting = await _unitOfWork.UserProfileRepository.GetByPhoneAsync(registerRequest.PhoneNumber);
            if (phoneExisting != null)
            {
                return Result.Failure(new Error("Auth.Register.Phone.Duplicate", "Phone is existed"));
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var userProfile = new UserProfile
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                Phone = registerRequest.PhoneNumber,
            };

            var account = new Account
            { 
                Username = registerRequest.UserName,
                PasswordHash = hashedPassword,
                AccountType = AccountType.Local,
                UserProfile = userProfile,
            };

            await _unitOfWork.UserProfileRepository.AddAsync(userProfile);
            await _unitOfWork.AccountRepository.AddAsync(account);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                /*return Result.Failure(new Error("Auth.Add.Database.Error", "Register failed. Please try again later."));*/
                return Result.Failure(new Error("Auth.Add.Database.Error", e.Message.ToString()));
            }
            return Result.Success();
        }
    }
}
