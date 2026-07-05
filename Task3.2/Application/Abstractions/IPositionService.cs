using Domain.Models;

namespace Application.Abstractions
{
    public interface IPositionService
    {
        Task<Position?> GetPositionByIdAsync(int id);
        Task<IEnumerable<Position>> GetAllPositionsAsync();
        Task CreatePositionAsync(Position position);
        Task UpdatePositionAsync(Position position);
        Task DeletePositionAsync(int id);
    }
}
