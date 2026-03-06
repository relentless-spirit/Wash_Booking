using BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Application.Abstractions.Services;

namespace Modules.Users.Domain.Entities
{
    public class User : Entity<Guid>, IAggregateRoot
    {
        //authentication
        public string Email { get; private set; } = null!;
        public string Username { get; private set; } = null!;
        public string? PasswordHash { get; private set; }
        public bool IsActive { get; private set; } = true;

        //information about the user
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }
        public string? Address { get; private set; }

        //để lưu trữ các refresh token liên quan đến user, giúp quản lý phiên đăng nhập và bảo mật. 
        //hỗ trợ việc user đăng nhập trên nhiều thiết bị hoặc trình duyệt khác nhau.
        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        //để lưu trữ các thông tin đăng nhập khác của user, như đăng nhập qua mạng xã hội (Google, Facebook, v.v.)
        private readonly List<UserLogin> _logins = new();
        public IReadOnlyCollection<UserLogin> Logins => _logins.AsReadOnly();

        private User() { }

        // Factory method to create a new User instance by registering with the system.
        public static User Create(
            string email,
            string username,
            string? passwordHash,
            string firstName,
            string lastName)
        {
            ArgumentException.ThrowIfNullOrEmpty(email);
            ArgumentException.ThrowIfNullOrEmpty(username);
            ArgumentNullException.ThrowIfNull(passwordHash);
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true
            };
            
            //sau này có thể thêm domain event UserRegisteredEvent vào đây nếu cần thiết
            
            return user;
        }

        // Factory method to create a new User instance by registering via a third-party social provider.
        public static User CreateSocial(
            string email,
            string username,
            string firstName, 
            string lastName, 
            string provider, 
            string providerKey)
        {
            ArgumentException.ThrowIfNullOrEmpty(email);
            ArgumentException.ThrowIfNullOrEmpty(provider);
            ArgumentNullException.ThrowIfNull(providerKey);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = email,
                PasswordHash = null,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true
            };
            
            user.AddLogin(provider, providerKey, username, email);

            return user;
        }
        
        public void UpdateProfile(string firstName, string lastName, string? address, string? phoneNumber, DateTime? modifiedOn)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PhoneNumber = phoneNumber;
            ModifiedOnUtc = modifiedOn;
        }
        
        public void AddRefreshToken(string token, DateTime expiresOnUtc)
        {
            ArgumentException.ThrowIfNullOrEmpty(token);
            if (expiresOnUtc <= DateTime.UtcNow) throw new ArgumentException("Expiration time must be in the future.", nameof(expiresOnUtc));

            _refreshTokens.RemoveAll(rt => !rt.IsActive);
            
            _refreshTokens.Add(RefreshToken.Create(token, expiresOnUtc));
        }
        
        private void AddLogin(string provider, string key, string username, string email)
        {
            if (!_logins.Any(x => x.Provider == provider && x.ProviderKey == key))
            {
                _logins.Add(UserLogin.Create(provider, key, username, email));
            }
        }


    }
}
