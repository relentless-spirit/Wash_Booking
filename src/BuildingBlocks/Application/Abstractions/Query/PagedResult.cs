namespace BuildingBlocks.Application.Abstractions.Query;

/// <summary>
/// Generic paginated result container.
/// Use PagedResult.Create() factory method to instantiate.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Internal constructor to enforce factory method usage.
    /// Only accessible within BuildingBlocks assembly.
    /// </summary>
    internal PagedResult(
        IReadOnlyCollection<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// The items in the current page (immutable).
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalCount / (double)PageSize)
        : 0;

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Maps the items to a different type while preserving pagination metadata.
    /// </summary>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="mapper">Function to map each item.</param>
    /// <returns>A new PagedResult with mapped items.</returns>
    /// <exception cref="ArgumentNullException">If mapper is null.</exception>
    public PagedResult<TDestination> Map<TDestination>(Func<T, TDestination> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        var mappedItems = Items.Select(mapper).ToList();

        // ✅ Use factory method to ensure validation
        return PagedResult.Create(
            mappedItems,
            TotalCount,
            PageNumber,
            PageSize);
    }
}

/// <summary>
/// Static factory class for creating PagedResult instances.
/// Provides type inference for cleaner syntax.
/// </summary>
public static class PagedResult
{
    /// <summary>
    /// Creates a new paginated result with validation.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="items">The items for the current page.</param>
    /// <param name="totalCount">Total count of items.</param>
    /// <param name="pageNumber">Current page number (1-based).</param>
    /// <param name="pageSize">Items per page.</param>
    /// <returns>A new PagedResult instance.</returns>
    /// <exception cref="ArgumentNullException">If items is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If validation fails.</exception>
    public static PagedResult<T> Create<T>(
        IEnumerable<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (totalCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalCount),
                "Total count cannot be negative");
        }

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                "Page number must be greater than 0");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be greater than 0");
        }

        // ✅ Optimize: Avoid double enumeration
        var readOnlyItems = items as IReadOnlyCollection<T> ?? items.ToList();

        return new PagedResult<T>(readOnlyItems, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Creates an empty paginated result.
    /// Useful for scenarios with no data.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="pageNumber">Current page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    /// <returns>An empty PagedResult instance.</returns>
    public static PagedResult<T> Empty<T>(int pageNumber = 1, int pageSize = 10)
    {
        // ✅ Reuse Create() to ensure validation
        return Create(Array.Empty<T>(), 0, pageNumber, pageSize);
    }
}