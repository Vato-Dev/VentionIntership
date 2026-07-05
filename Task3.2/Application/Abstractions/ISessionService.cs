using Domain.Models;

namespace Application.Abstractions
{
    public interface ISessionService
    {
        Task<Session?> GetSessionByIdAsync(int id);
        Task<IEnumerable<Session>> GetAllSessionsAsync();
        Task CreateSessionAsync(Session session);
        Task UpdateSessionAsync(Session session);
        Task DeleteSessionAsync(int id);
    }
}
