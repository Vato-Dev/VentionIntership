using Task1._6.DTOs;

namespace Task1._6.Abstractions
{
    public interface IBookService
    {
        IEnumerable<BookDto> GetAll();
        BookDto? GetById(int id);
        BookDto Create(CreateBookDto dto);
        bool Update(int id, UpdateBookDto dto);
        bool Delete(int id);
    }
}
