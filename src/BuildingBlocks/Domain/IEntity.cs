namespace BuildingBlocks.Domain;

public interface IEntity
{
    // 1. Để lấy danh sách Domain Events mà không cần quan tâm ID là int hay Guid
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    // 2. Để clear events sau khi save xong
    void ClearDomainEvents();
}