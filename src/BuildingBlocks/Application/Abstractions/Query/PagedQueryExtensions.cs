namespace BuildingBlocks.Application.Abstractions.Query;

/// <summary>
/// Extension methods for IPagedQuery to calculate pagination parameters.
/// </summary>
public static class PagedQueryExtensions
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    /// <summary>
    /// Gets the number of items to skip for pagination.
    /// </summary>
    public static int GetSkip(this IPagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var pageNumber = GetValidatedPageNumber(query.PageNumber);
        var pageSize = GetValidatedPageSize(query.PageSize);
        
        return (pageNumber - 1) * pageSize;
    }

    /// <summary>
    /// Gets the number of items to take for pagination.
    /// </summary>
    public static int GetTake(this IPagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return GetValidatedPageSize(query.PageSize);
    }

    /// <summary>
    /// Gets whether sorting is in descending order.
    /// </summary>
    public static bool IsDescending(this IPagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return query.SortOrder == SortOrder.Descending;
    }

    /// <summary>
    /// Gets the validated page number (ensures >= 1).
    /// </summary>
    public static int GetValidatedPageNumber(this IPagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return GetValidatedPageNumber(query.PageNumber);
    }

    /// <summary>
    /// Gets the validated page size (ensures between 1 and MaxPageSize).
    /// </summary>
    public static int GetValidatedPageSize(this IPagedQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return GetValidatedPageSize(query.PageSize);
    }

    // Private helpers
    private static int GetValidatedPageNumber(int pageNumber)
    {
        return pageNumber < 1 ? DefaultPageNumber : pageNumber;
    }

    private static int GetValidatedPageSize(int pageSize)
    {
        if (pageSize < 1) return DefaultPageSize;
        if (pageSize > MaxPageSize) return MaxPageSize;
        return pageSize;
    }
}