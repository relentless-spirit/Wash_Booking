using BuildingBlocks.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RefreshToken> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("RefreshTokens", "users");
        
        builder.Property(rt => rt.Token).HasMaxLength(200).IsRequired();
        
        builder.Ignore(rt => rt.IsActive);
        
        builder.HasOne<User>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}