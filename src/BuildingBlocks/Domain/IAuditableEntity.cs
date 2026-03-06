namespace BuildingBlocks.Domain;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; }
    DateTime? ModifiedOnUtc { get; }
}