namespace BuildingBlocks.Application.Abstractions.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
