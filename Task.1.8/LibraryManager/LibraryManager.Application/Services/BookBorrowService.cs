using LibraryManager.Application.Interfaces;
using LibraryManager.Domain.Models;

namespace LibraryManager.Application.Services
{
    public sealed class BookBorrowService(
        IBaseRepository<Book> bookRepository, 
        IBaseRepository<Reader> ReaderRepository, 
        IBorrowingRepository borrowRepository, 
        IUnitOfWork unitOfWork) : IBookBorrowService
    {
        public async Task BorrowBook(int BookId, int ReaderId, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(BookId,cancellationToken);
            if(book == null) 
                throw new Exception("Book not found");
            
            var reader = await ReaderRepository.GetByIdAsync(ReaderId,cancellationToken);
            if(reader == null) 
                throw new Exception("Reader not found");
            
            if (!book.IsAvailable)
                throw new Exception("Book is not available");
                
            if (reader.Status != ReaderState.Active)
                throw new Exception("User is not Allowed To This Action");
            
            book.IsAvailable = false;
            
            var isTaken = await borrowRepository.IsBorrowed(BookId, ReaderId, cancellationToken);
            if (isTaken)
                throw new Exception("This Book is already Borrowed");
            
            var borrowBook = new BookBorrowing
            {
                BookId = book.Id,
                ReaderId = reader.Id,
                IsReturned = false,
                BorrowedAt = DateTime.Now,
                ReturnedAt = null
            };
            
            borrowRepository.Add(borrowBook);
            bookRepository.Update(book);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ReturnBook(int BorrowingId, DateTime ReturnDate, string? aboutCondition, BookCondition ConditionAtReturn, CancellationToken cancellationToken)
        {
            var borrowing = await borrowRepository.GetBorrowingWithBookByIdAsync(BorrowingId,cancellationToken);
            if (borrowing == null)
                throw new Exception("Borrowing not found");
            
            var book = borrowing.Book;

            book.Condition = ConditionAtReturn;
            book.IsAvailable = ConditionAtReturn != BookCondition.WellWorn;
            
            if (ConditionAtReturn == BookCondition.WellWorn)
                borrowing.AddFine(borrowing.Id,aboutCondition?? "Not Acceptable Condition", 10 );
            if (borrowing.BorrowedAt.AddDays(14) < ReturnDate)
                borrowing.AddFine(borrowing.Id,"Late Returnal", 5 );
            

            borrowing.IsReturned = true;
            borrowing.ReturnedAt = ReturnDate;
            
            borrowRepository.Update(borrowing);
            bookRepository.Update(book);//it will do automatically I just like to write it to make it more obvious
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
