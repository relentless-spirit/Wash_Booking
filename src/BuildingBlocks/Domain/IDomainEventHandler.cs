using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Domain;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Naming convention for DDD")]
public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}
