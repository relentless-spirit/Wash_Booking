using BuildingBlocks.Domain;

namespace Modules.Users.Domain.Entities
{
    public class RefreshToken : Entity<Guid>
    {
        public string Token { get; private set; } = null!;
        public DateTime ExpiresOnUtc { get; private set; }
        public DateTime? RevokedOnUtc { get; private set; }
        
        public Guid UserId { get; private set; }

        // Thuộc tính tính toán (Computed Property) để check nhanh
        public bool IsActive => RevokedOnUtc is null && ExpiresOnUtc > DateTime.UtcNow;

        private RefreshToken() { }
        
        internal static RefreshToken Create(string token, DateTime expiresOnUtc)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                ExpiresOnUtc = expiresOnUtc,
                CreatedOnUtc = DateTime.UtcNow
            };
        } 
        
        public void Revoke()
        {
            RevokedOnUtc = DateTime.UtcNow;
        }
    }
}
