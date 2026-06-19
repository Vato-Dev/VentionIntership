namespace LibraryManager.Domain.Models
{
    public sealed class Catalogue
    {
        public int Id { get; set; }
        public int? ParentId { get; set;}
        public string Name { get; set; }
        
        private readonly List<Book> _bookCopies = new();

        public IReadOnlyCollection<Book> BookCopies => _bookCopies.AsReadOnly();

        public int TotalQuantity => _bookCopies.Count;
        public int AvailableQuantity => _bookCopies.Count(c => c.IsAvailable); 
        public int AcceptableConditionCount => _bookCopies.Count(c => c.Condition != BookCondition.WellWorn);
    }
}
