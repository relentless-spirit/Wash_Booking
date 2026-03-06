using BuildingBlocks.Domain;
using BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
namespace BuildingBlocks.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity, TEntityId> : IGenericRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>, IAggregateRoot
    where TEntityId : notnull
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    
    protected ApplicationDbContext Context => _context;
    protected DbSet<TEntity> DbSet => _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    public void Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Remove(entity);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id), ct);
    }

    public virtual async Task<bool> ExistsAsync(TEntityId id, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _dbSet.AnyAsync(e => e.Id.Equals(id), ct);
    }
}