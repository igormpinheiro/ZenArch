using SharedKernel.Common.Pagination;

namespace Web.API.Contracts.Requests;

public sealed record PaginationRequest(int Page, int PageSize, string SortBy, bool SortDescending) : IPaginationRequest;