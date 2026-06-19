using Task1._6.Abstractions;
using Task1._6.DTOs;
using Task1._6.Models;

namespace Task1._6.Services
{
    public class BookService : IBookService
    {
        private readonly List<Book> _booksDb = new();
        private int _nextId = 1;

        public BookService()
        {
            _booksDb.Add(new Book
            {
                Id = _nextId++,
                Title = "Doing Intership Task",
                Author = "Strugatsky brothers",
                Year = DateTime.UtcNow.Year,
                Price = 100
            });
        }

        public IEnumerable<BookDto> GetAll()
        {
            return _booksDb.Select(b => new BookDto(b.Id, b.Title, b.Author, b.Year, b.Price)); //Usually i use Mapster , for learning tasks it's overkill same to concurrency dictionary even if i add more logic
        }

        public BookDto? GetById(int id)
        {
            var b = _booksDb.FirstOrDefault(b => b.Id == id);
            if (b is null) return null;
        
            return new BookDto(b.Id, b.Title, b.Author, b.Year, b.Price);
        }

        public BookDto Create(CreateBookDto dto)
        {
            var newBook = new Book
            {
                Id = _nextId++,
                Title = dto.Title,
                Author = dto.Author,
                Year = dto.Year,
                Price = dto.Price
            };
        
            _booksDb.Add(newBook);
        
            return new BookDto(newBook.Id, newBook.Title, newBook.Author, newBook.Year, newBook.Price);
        }

        public bool Update(int id, UpdateBookDto dto)
        {
            var existingBook = _booksDb.FirstOrDefault(b => b.Id == id);
            if (existingBook is null) return false;

            existingBook.Title = dto.Title;
            existingBook.Author = dto.Author;
            existingBook.Year = dto.Year;
            existingBook.Price = dto.Price;

            return true;
        }

        public bool Delete(int id)
        {
            var book = _booksDb.FirstOrDefault(b => b.Id == id);
            if (book is null) return false;

            _booksDb.Remove(book);
            return true;
        }
    }
}
