namespace BuildingBlocks.Domain;

public interface IGenericRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>, IAggregateRoot
    where TEntityId : notnull
{
    void Add(TEntity entity);
    void Remove(TEntity entity);
    Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken ct);
    Task<bool> ExistsAsync(TEntityId id, CancellationToken ct);
}