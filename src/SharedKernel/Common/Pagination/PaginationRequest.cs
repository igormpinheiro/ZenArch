namespace SharedKernel.Common.Pagination;

public sealed record PaginationRequest : IPaginationRequest
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    private const int MinPageSize = 1;

    private int _page = 1;
    private int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        init => _page = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, MinPageSize, MaxPageSize);
    }

    public string SortBy { get; init; } = "Id";
    public bool SortDescending { get; init; }

    public static PaginationRequest Create(int page = 1, int pageSize = DefaultPageSize, 
        string sortBy = "Id", bool sortDescending = false)
    {
        return new PaginationRequest
        {
            Page = page,
            PageSize = pageSize,
            SortBy = string.IsNullOrWhiteSpace(sortBy) ? "Id" : sortBy,
            SortDescending = sortDescending
        };
    }
}