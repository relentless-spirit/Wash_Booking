using BuildingBlocks.Infrastructure.Persistence.Database;
using BuildingBlocks.Infrastructure.Persistence.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Application.Services;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Infrastructure.Persistence.DBContext;

internal sealed class UserDbContext(DbContextOptions<UserDbContext> options, IDomainEventsDispatcher domainEventsDispatcher) 
    : ApplicationDbContext(options, domainEventsDispatcher),
        IUsersUnitOfWork
{
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("users");
        modelBuilder.HasPostgresExtension("pgcrypto");
    }
    
}