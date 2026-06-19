using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Application.Services;

namespace LibraryManager.Application.Interfaces.Books
{
    public interface IBookService
    {
        Task AddBook(AddBookRequest bookRequest, CancellationToken cancellationToken);
        Task UpdateIsbn(int bookId, string newIsbn, CancellationToken cancellationToken);
        Task UpdateBookInfo(UpdateBookRequest updateBookRequest, CancellationToken cancellationToken);
        Task<BookInfoDto> GetBookInfo(int bookId, CancellationToken cancellationToken);
    }
}
