namespace Application.DTOs
{
    public sealed class PagedResponse<T>
    {
        public IReadOnlyCollection<T> Data { get; init; } = [];
        public int? PageNumber { get; init; }
        public int PageSize { get; init; }
        public int? TotalItems { get; init; }
        public int? TotalPages { get; init; }
        public int? LastSeenId { get; init; }
    }
}
