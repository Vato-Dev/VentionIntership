using LibraryManager.Domain.Models;

namespace LibraryManager.Application.Interfaces
{
    public interface IBookBorrowService
    {
        Task BorrowBook(int BookId, int ReaderId, CancellationToken cancellationToken);
        Task ReturnBook(int BorrowingId, DateTime ReturnDate, string? aboutCondition, BookCondition ConditionAtReturn, CancellationToken cancellationToken);
    }
}
