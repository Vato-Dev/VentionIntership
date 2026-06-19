namespace LibraryManager.Domain.Models
{
    public sealed class Reader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PersonalNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime RegisteredAt { get; set; }
        public ReaderState Status { get; set; }

        private readonly List<BookBorrowing> _bookBorrowings = new();
        public IReadOnlyCollection<BookBorrowing> BookBorrowings => _bookBorrowings.AsReadOnly();
    }
    public enum ReaderState
    {
        Active,
        DeletedByUser,
        Blocked
    }
}
