using BuildingBlocks.Application.Abstractions.Services;

namespace BuildingBlocks.Infrastructure.Persistence.Times;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
