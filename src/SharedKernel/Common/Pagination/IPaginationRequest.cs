namespace SharedKernel.Common.Pagination;

public interface IPaginationRequest
{
    int Page { get; }
    int PageSize { get; }
    string SortBy { get; }
    bool SortDescending { get; }
}
