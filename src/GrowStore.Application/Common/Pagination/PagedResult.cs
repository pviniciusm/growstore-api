namespace GrowStore.Application.Common.Pagination;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int Limit { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => Limit > 0 ? (int)Math.Ceiling(TotalCount / (double)Limit) : 0;

    public static PagedResult<T> Create(IEnumerable<T> items, int page, int limit, int totalCount)
        => new()
        {
            Items = items,
            Page = page,
            Limit = limit,
            TotalCount = totalCount
        };
}
