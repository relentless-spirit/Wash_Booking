using BuildingBlocks.Application.Abstractions.Data;
using BuildingBlocks.Domain;
using BuildingBlocks.Infrastructure.Persistence.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence.Database;

public abstract class ApplicationDbContext(
    DbContextOptions options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IUnitOfWork 
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        
        // Quét thêm cấu hình chung nếu có (VD: Outbox)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Logic xử lý Domain Events trước/sau khi lưu
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList(); // Copy ra list mới
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();
            
        // Dispatcher này sẽ bắn event sang MediatR handler
        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}