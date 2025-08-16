namespace SharedKernel.Common.Pagination;

public sealed record PaginationMetadata : IPaginationMetadata
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public int CurrentPageCount { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
    public bool IsFirstPage { get; init; }
    public bool IsLastPage { get; init; }

    public static PaginationMetadata Create(IPaginationRequest request, int totalCount, int currentPageCount)
    {
        var totalPages = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)request.PageSize) : 0;
        var currentPage = Math.Min(request.Page, Math.Max(1, totalPages));

        return new PaginationMetadata
        {
            Page = currentPage,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPageCount = currentPageCount,
            HasPreviousPage = currentPage > 1,
            HasNextPage = currentPage < totalPages,
            IsFirstPage = currentPage <= 1,
            IsLastPage = currentPage >= totalPages || totalPages == 0
        };
    }
}