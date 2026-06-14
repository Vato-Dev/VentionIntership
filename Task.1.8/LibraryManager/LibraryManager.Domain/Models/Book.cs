using LibraryManager.Domain.ValueObjects;

namespace LibraryManager.Domain.Models
{
    public sealed class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required Isbn Isbn { get; set; }
        public string AuthorName { get; set; }
        public int PublishYear { get; set; }
        public int CatalogueId { get; set; }
        public BookCondition Condition { get; set; }
        public bool IsAvailable { get; set; } = true;
        
        public Catalogue Catalogue { get; set; }
        public Book() //mapster can't work without public ctor properly i'm tired to use reflection to give it access
        {
        } 

        public static Book Create(string title, string isbn, string authorName, int publishYear, int catalogueId)
            =>
                new()
                {
                    Title = title,
                    Isbn = Isbn.Create(isbn),
                    AuthorName = authorName,
                    PublishYear = publishYear,
                    CatalogueId = catalogueId,
                    Condition = BookCondition.New,
                    IsAvailable = true
                };

        public void UpdateBookIsbn(string isbn)
         => Isbn = Isbn.Create(isbn);
        
    }
}
