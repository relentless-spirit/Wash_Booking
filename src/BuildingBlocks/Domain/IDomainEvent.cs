using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace BuildingBlocks.Domain;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface IDomainEvent : INotification
{
}
