namespace LibraryManager.Domain.Models
{
    
    public sealed class Fine
    {
        public int Id { get; set; }
        public int BorrowingId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime GaveAt { get; set; }
        public bool IsPaid { get; set; }
        
        public BookBorrowing Borrowing { get; set; }
        internal Fine()
        {
            
        }
        public static Fine Create(int borrowingId, string description, decimal amount)
            => new()
            {
                BorrowingId = borrowingId,
                Description = description,
                Amount = amount,
                GaveAt = DateTime.UtcNow,
                IsPaid = false
            };
    }
}
