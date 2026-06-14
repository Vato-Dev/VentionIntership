using LibraryManager.Application.Services;

namespace LibraryManager.Application.Interfaces
{
    public interface IBookQuerries
    {
        Task<BookInfoDto?> GetByIdAsync(int id, CancellationToken ct);
    }
}
