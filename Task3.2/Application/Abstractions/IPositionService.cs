using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IPositionService
    {
        Task<Position?> GetPositionByIdAsync(int id);
        Task<PagedResponse<Position>> GetAllPositionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10);
        Task CreatePositionAsync(Position position);
        Task UpdatePositionAsync(Position position);
        Task DeletePositionAsync(int id);
    }
}
