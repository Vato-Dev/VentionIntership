using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    public sealed class OrganizationService(
        IBaseRepository<Organization> organizationRepository, 
        IUnitOfWork unitOfWork,
        IMemoryCache cache) : IOrganizationService
    {
        private const string CacheKeyPrefix = "org_";
        private const string AllOrgsCacheKey = "all_orgs_paged_";

        public async Task<Organization?> GetOrganizationByIdAsync(CancellationToken cancellationToken,int id)
        {
            string cacheKey = $"{CacheKeyPrefix}{id}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await organizationRepository.GetByIdAsync(id, cancellationToken);
            });
        }

        public async Task<PagedResponse<Organization>> GetAllOrganizationsAsync(
            CancellationToken cancellationToken, int? keySetId = null, int? page = 1, int? pageSize = 10)
        {
            string cacheKey = $"{AllOrgsCacheKey}{keySetId}_{page}_{pageSize}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                return await organizationRepository.GetAllAsync(cancellationToken, keySetId, page, pageSize);
            }) ?? new PagedResponse<Organization>();
        }

        public async Task CreateOrganizationAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await organizationRepository.AddAsync(organization, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                ClearCache();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdateOrganizationAsync(Organization organization, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                organizationRepository.Update(organization);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                cache.Remove($"{CacheKeyPrefix}{organization.Id}");
                ClearCache();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task DeleteOrganizationAsync(int id, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var organization = await organizationRepository.GetByIdAsync(id, cancellationToken);
                if (organization != null)
                {
                    organizationRepository.Delete(organization);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                cache.Remove($"{CacheKeyPrefix}{id}");
                ClearCache();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        private void ClearCache()
        {
            //For finer-grained invalidation with complex dynamic queries better approach is Compact/CancellationChangeToken pattern
            cache.Remove(AllOrgsCacheKey); 
        }
    }
}