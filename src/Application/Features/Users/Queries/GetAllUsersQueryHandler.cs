using Application.Abstractions.Messaging;
using Application.Mappings;
using Application.Models;
using Domain.Interfaces.Repositories;
using ErrorOr;
using SharedKernel.Common.Pagination;

namespace Application.Features.Users.Queries;

public sealed record GetAllUsersQuery(IPaginationRequest Pagination)
    : IQuery<PaginatedResult<UserViewModel>>;

internal sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetAllUsersQuery, PaginatedResult<UserViewModel>>
{
    public async Task<ErrorOr<PaginatedResult<UserViewModel>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await userRepository.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return PaginatedResult<UserViewModel>.Empty(request.Pagination);
        }

        var skip = (request.Pagination.Page - 1) * request.Pagination.PageSize;

        var users = await userRepository.GetPaginatedAsync(
            skip,
            request.Pagination.PageSize,
            request.Pagination.SortBy,
            request.Pagination.SortDescending,
            cancellationToken);

        var userViewModels = users.ToViewModel();

        var paginatedResult = PaginatedResult<UserViewModel>.Create(
            userViewModels,
            request.Pagination,
            totalCount);
        return paginatedResult;
    }
}
