using BuildingBlocks.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Infrastructure.Persistence.Configurations;

public class UserLoginConfiguration : BaseEntityConfiguration<UserLogin, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserLogin> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("UserLogins", "users");
        
        builder.Property(ul => ul.Provider).HasMaxLength(100).IsRequired();
        builder.Property(ul => ul.ProviderKey).HasMaxLength(200).IsRequired();
        builder.Property(ul => ul.ProviderUserName).HasMaxLength(200).IsRequired();
        builder.Property(ul => ul.ProviderEmail).HasMaxLength(200).IsRequired();
        
        builder.HasIndex(ul => new { ul.Provider, ul.ProviderKey }).IsUnique();
        
        builder.HasOne<User>()
            .WithMany(u => u.Logins)
            .HasForeignKey(ul => ul.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}