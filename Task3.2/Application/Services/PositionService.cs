
using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    public sealed class PositionService(
        IBaseRepository<Position> positionRepository, 
        IUnitOfWork unitOfWork,
        IMemoryCache cache) : IPositionService
    {
        private const string CacheKeyPrefix = "pos_";
        private const string AllPositionsCacheKey = "all_positions_paged_";

        public async Task<Position?> GetPositionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{CacheKeyPrefix}{id}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await positionRepository.GetByIdAsync(id, cancellationToken);
            });
        }

        public async Task<PagedResponse<Position>> GetAllPositionsAsync(
            int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{AllPositionsCacheKey}{keySetId}_{page}_{pageSize}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await positionRepository.GetAllAsync(cancellationToken, keySetId, page, pageSize);
            }) ?? new PagedResponse<Position>();
        }

        public async Task CreatePositionAsync(Position position, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await positionRepository.AddAsync(position, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                cache.Remove(AllPositionsCacheKey);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdatePositionAsync(Position position, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                positionRepository.Update(position);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                cache.Remove($"{CacheKeyPrefix}{position.Id}");
                cache.Remove(AllPositionsCacheKey);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task DeletePositionAsync(int id, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var position = await positionRepository.GetByIdAsync(id, cancellationToken);
                if (position != null)
                {
                    positionRepository.Delete(position);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                cache.Remove($"{CacheKeyPrefix}{id}");
                cache.Remove(AllPositionsCacheKey);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }
    }
}