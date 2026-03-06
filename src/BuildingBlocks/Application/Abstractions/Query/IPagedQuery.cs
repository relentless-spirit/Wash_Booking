namespace BuildingBlocks.Application.Abstractions.Query;

/// <summary>
/// Base interface for queries that support pagination and sorting.
/// Combine both concerns as they are almost always used together.
/// </summary>
public interface IPagedQuery
{
    /// <summary>
    /// Page number (1-based). Default is 1.
    /// </summary>
    int PageNumber { get; init; }
    
    /// <summary>
    /// Number of items per page. Default is 10, max is 100.
    /// </summary>
    int PageSize { get; init; }
    
    /// <summary>
    /// Field name to sort by (e.g., "CreatedAt", "Name").
    /// If null, repository will use default sorting.
    /// </summary>
    string? SortBy { get; init; }
    
    /// <summary>
    /// Sort direction. Default is Ascending.
    /// </summary>
    SortOrder SortOrder { get; init; }
}