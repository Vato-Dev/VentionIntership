using System.Data;
using Application.Abstractions;
using Domain.Models;

namespace Application.Services
{
    public sealed class PositionService(IBaseRepository<Position> positionRepository, IUnitOfWork unitOfWork) : IPositionService
    {
        public Task<Position?> GetPositionByIdAsync(int id) => positionRepository.GetByIdAsync(id);

        public Task<IEnumerable<Position>> GetAllPositionsAsync() => positionRepository.GetAllAsync();

        public async Task CreatePositionAsync(Position position)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                await positionRepository.AddAsync(position);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdatePositionAsync(Position position)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                positionRepository.Update(position);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeletePositionAsync(int id)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var position = await positionRepository.GetByIdAsync(id);
                if (position != null)
                {
                    positionRepository.Delete(position);
                    await unitOfWork.SaveChangesAsync();
                }
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}