namespace SharedKernel.Common.Pagination;

public sealed record PaginationRequest : IPaginationRequest
{
    private const int _maxPageSize = 100;
    private const int _defaultPageSize = 10;
    private const int _minPageSize = 1;

    private int _page = 1;
    private int _pageSize = _defaultPageSize;

    public int Page
    {
        get => _page;
        init => _page = Math.Max(1, value);
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = Math.Clamp(value, _minPageSize, _maxPageSize);
    }

    public string SortBy { get; init; } = "Id";
    public bool SortDescending { get; init; }

    public static PaginationRequest Create(int page = 1, int pageSize = _defaultPageSize, 
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