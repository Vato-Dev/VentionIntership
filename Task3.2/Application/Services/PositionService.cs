using Application.Abstractions;
using Domain.Models;

namespace Application.Services
{
    public sealed class PositionService : IPositionService
    {
        private readonly IBaseRepository<Position> _positionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PositionService(IBaseRepository<Position> positionRepository, IUnitOfWork unitOfWork)
        {
            _positionRepository = positionRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<Position?> GetPositionByIdAsync(int id) => _positionRepository.GetByIdAsync(id);

        public Task<IEnumerable<Position>> GetAllPositionsAsync() => _positionRepository.GetAllAsync();

        public async Task CreatePositionAsync(Position position)
        {
            await _positionRepository.AddAsync(position);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePositionAsync(Position position)
        {
            _positionRepository.Update(position);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePositionAsync(int id)
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if (position != null)
            {
                _positionRepository.Delete(position);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
