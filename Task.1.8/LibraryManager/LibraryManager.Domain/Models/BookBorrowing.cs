namespace LibraryManager.Domain.Models
{
    public sealed class BookBorrowing
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public bool IsReturned { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        
        //Nav Props to not write Joins 
        public Book Book { get; set; } 
        public Reader Reader { get; set; }
        
        private List<Fine> _fines = new();
        public IReadOnlyCollection<Fine> Fines => _fines.AsReadOnly();

        public void AddFine(int borrowingId,string description, decimal amount)
        {
            var fine = Fine.Create(borrowingId, description, amount);
            _fines.Add(fine);
        }
    }
}
