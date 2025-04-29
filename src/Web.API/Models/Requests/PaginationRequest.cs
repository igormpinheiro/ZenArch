namespace Web.API.Models.Responses;

public class PaginationRequest
{
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 10;

    private int _pageSize = DefaultPageSize;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string SortBy { get; set; } = "Id";
    public bool SortDescending { get; set; }
}
