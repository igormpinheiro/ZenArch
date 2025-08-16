namespace SharedKernel.Common.Pagination;

public interface IPaginatedResult<out T>
{
    IReadOnlyList<T> Items { get; }
    IPaginationMetadata Metadata { get; }
}