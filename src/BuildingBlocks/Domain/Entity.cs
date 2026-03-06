using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Domain;

public abstract class Entity<TId> : IEntity, IAuditableEntity
{
    public TId Id { get; init; } = default!;
    
    public DateTime CreatedOnUtc { get; set; } 
    public DateTime? ModifiedOnUtc { get; set; }
    
    protected Entity(TId id)
    {
        Id = id;
    }
    
    protected Entity()
    {
    }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Raise is a standard term in DDD for publishing domain events.")]
    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
