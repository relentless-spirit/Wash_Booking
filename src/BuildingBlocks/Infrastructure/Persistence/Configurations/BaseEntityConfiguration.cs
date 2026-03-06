using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Persistence.Configurations;

public abstract class BaseEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TId>
    where TId : notnull
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.HasKey(e => e.Id);
        

        builder.Property(e => e.CreatedOnUtc)
            .IsRequired(); 

        builder.Property(e => e.ModifiedOnUtc)
            .IsRequired(false); // Ngày sửa có thể null
            
        // 3. (Tuỳ chọn) Config Soft Delete nếu có (IsDeleted)
        // builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        // builder.HasQueryFilter(e => !e.IsDeleted);
        
        ConfigureEntity(builder);
    }
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}