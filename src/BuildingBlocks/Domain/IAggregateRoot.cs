using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Domain;

/// <summary>
/// This is a "Marker Interface".
/// It has no methods, but it tells the Compiler: "This Entity is a BOSS".
/// We use this to ensure only Aggregates can have Repositories.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface IAggregateRoot
{
}