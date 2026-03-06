using BuildingBlocks.Domain;

namespace Modules.Users.Domain.Entities
{
    public class UserLogin : Entity<Guid>
    {
        public string Provider { get; private set; } = null!;
        public string ProviderKey { get; private set; } = null!;
        public string ProviderUserName { get; private set; } = null!;
        public string ProviderEmail { get; private set; } = null!;
        public Guid UserId { get; private set; }
        
        private UserLogin() { }
        
        internal static UserLogin Create(
            string provider,
            string providerKey,
            string providerUserName,
            string providerEmail)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(provider);
            ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(providerUserName);
            ArgumentException.ThrowIfNullOrWhiteSpace(providerEmail);
            
            var userLogin = new UserLogin
            {
                Id = Guid.NewGuid(),
                Provider = provider,
                ProviderKey = providerKey,
                ProviderUserName = providerUserName,
                ProviderEmail = providerEmail
            };

            return userLogin;
        }
    }
}
