namespace BuildingBlocks.Domain;

public interface IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    void ClearDomainEvents();
}
