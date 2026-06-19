using LibraryManager.Domain.Models;

namespace LibraryManager.Application.Interfaces
{
    public interface IBorrowingRepository : IBaseRepository<BookBorrowing>
    {
        Task<BookBorrowing?> GetBorrowingWithBookByIdAsync(int borrowingId, CancellationToken ct);
        Task<bool> IsBorrowed(int bookId, int readerId, CancellationToken ct);
    }
}
