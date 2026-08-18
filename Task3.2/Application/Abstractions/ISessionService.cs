using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface ISessionService
    {
        Task<SessionResponseDto?> GetSessionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResponse<SessionResponseDto, int>> GetAllSessionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);
        Task<SessionResponseDto> CreateSessionAsync(SessionCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateSessionAsync(int id, SessionUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteSessionAsync(int id, CancellationToken cancellationToken = default);
    }
}
