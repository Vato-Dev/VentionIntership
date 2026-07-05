using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface ISessionService
    {
        Task<Session?> GetSessionByIdAsync(int id);
        Task<PagedResponse<Session>> GetAllSessionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10);
        Task CreateSessionAsync(Session session);
        Task UpdateSessionAsync(Session session);
        Task DeleteSessionAsync(int id);
    }
}
