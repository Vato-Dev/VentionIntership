using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface ISessionService
    {
        Task<Session?> GetSessionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResponse<Session>> GetAllSessionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);
        Task CreateSessionAsync(Session session, CancellationToken cancellationToken = default);
        Task UpdateSessionAsync(Session session, CancellationToken cancellationToken = default);
        Task DeleteSessionAsync(int id, CancellationToken cancellationToken = default);
    }
}
