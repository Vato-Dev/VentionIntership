using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Application.Interfaces;
using LibraryManager.Application.Interfaces.Books;
using LibraryManager.Domain.Models;
using Mapster;

namespace LibraryManager.Application.Services
{
    public sealed class BookService(IBaseRepository<Book> bookRepository  , IBookQuerries bookQuerries, IUnitOfWork unitOfWork) : IBookService
    {
        public async Task AddBook(AddBookRequest bookRequest , CancellationToken cancellationToken)
        {
            var book = Book.Create(bookRequest.Title, bookRequest.Isbn, bookRequest.AuthorName,bookRequest.PublishYear, bookRequest.CatalogueId);
            bookRepository.Add(book);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateIsbn(int bookId, string newIsbn, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(bookId, cancellationToken);
            if (book == null)
                throw new Exception("Book not found");

            book.UpdateBookIsbn(newIsbn);
            
            bookRepository.Update(book);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateBookInfo(UpdateBookRequest updateBookRequest, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetByIdAsync(updateBookRequest.BookId, cancellationToken);
            if (book == null)
                throw new Exception("Book not found");
            
            var updated = updateBookRequest.Adapt(book);
            
            bookRepository.Update(updated);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task<BookInfoDto> GetBookInfo(int bookId,CancellationToken cancellationToken)
        {
            var book = await bookQuerries.GetByIdAsync(bookId, cancellationToken);
            return book == null ? throw new Exception("Book not found") : book;
        }
    }
    

    public sealed record AddBookRequest(string Title, string Isbn, string AuthorName, int PublishYear, int CatalogueId);
    public sealed record BookInfoDto(string Title, string Isbn, string AuthorName, int PublishYear, string CatalogueName,BookCondition Condition);
}
