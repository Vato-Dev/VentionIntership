using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IPositionService
    {
        Task<Position?> GetPositionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResponse<Position>> GetAllPositionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);
        Task CreatePositionAsync(Position position, CancellationToken cancellationToken = default);
        Task UpdatePositionAsync(Position position, CancellationToken cancellationToken = default);
        Task DeletePositionAsync(int id, CancellationToken cancellationToken = default);
    }
}
