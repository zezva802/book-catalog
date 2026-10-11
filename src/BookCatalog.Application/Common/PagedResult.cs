namespace BookCatalog.Application.Common;

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}