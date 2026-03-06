using BuildingBlocks.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Infrastructure.Persistence.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("Users", "users");
        
        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.Username).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();
        
        builder.Property(u => u.PasswordHash).HasMaxLength(60);
        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(15).IsRequired();
        builder.Property(u => u.Address).HasMaxLength(500);
        
        builder.Property(u => u.IsActive).HasDefaultValue(true);
    }
}