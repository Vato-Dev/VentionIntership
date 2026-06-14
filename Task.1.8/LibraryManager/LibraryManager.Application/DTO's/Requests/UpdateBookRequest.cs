using LibraryManager.Domain.Models;

namespace LibraryManager.Application.DTO_s.Requests
{
    public sealed class UpdateBookRequest 
    {
        public int BookId { get; set; }
        public string? Title { get; init; }
        public string? AuthorName { get; init; }
        public int? PublishYear { get; init; }
        public int? CatalogueId { get; init; }
        public BookCondition? Condition { get; init; }
    }
}
