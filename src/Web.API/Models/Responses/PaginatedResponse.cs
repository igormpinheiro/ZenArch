namespace Web.API.Models.Responses;

internal class PaginatedResponse<T>(List<T> items, int page, int pageSize, int totalCount)
{
    public List<T> Items { get; } = items;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PaginatedResponse<T> Create(List<T> items, PaginationRequest request, int totalCount)
    {
        return new PaginatedResponse<T>(items, request.Page, request.PageSize, totalCount);
    }
}
