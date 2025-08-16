namespace SharedKernel.Common.Pagination;

public sealed record PaginatedResult<T> : IPaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public IPaginationMetadata Metadata { get; init; } = new PaginationMetadata();

    public static PaginatedResult<T> Create(
        IEnumerable<T> items, 
        IPaginationRequest request, 
        int totalCount)
    {
        var itemsList = items?.ToList() ?? [];
        
        return new PaginatedResult<T>
        {
            Items = itemsList.AsReadOnly(),
            Metadata = PaginationMetadata.Create(request, totalCount, itemsList.Count)
        };
    }

    public static PaginatedResult<T> Empty(IPaginationRequest request)
    {
        return new PaginatedResult<T>
        {
            Items = [],
            Metadata = PaginationMetadata.Create(request, 0, 0)
        };
    }
}