using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    public sealed class OrganizationService(
        IBaseRepository<Organization, Guid> organizationRepository, 
        IUnitOfWork unitOfWork,
        IMemoryCache cache) : IOrganizationService
    {
        private const string CacheKeyPrefix = "org_";
        private const string AllOrgsCacheKey = "all_orgs_paged_";

        public async Task<OrganizationResponseDto?> GetOrganizationByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{CacheKeyPrefix}{id}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var org = await organizationRepository.GetByIdAsync(id, cancellationToken);
                
                return org == null ? null : new OrganizationResponseDto
                {
                    Id = org.Id.ToString(),
                    Name = org.Name,
                    CreatedAt = org.CreatedAt,
                    UpdatedAt = org.UpdatedAt
                };
            });
        }

        public async Task<PagedResponse<OrganizationResponseDto, Guid>> GetAllOrganizationsAsync(
            Guid? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{AllOrgsCacheKey}{keySetId}_{page}_{pageSize}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                
                var pagedEntities = await organizationRepository.GetAllAsync(cancellationToken, keySetId, page, pageSize);
                
                var mappedData = pagedEntities.Data.Select(org => new OrganizationResponseDto
                {
                    Id = org.Id.ToString(),
                    Name = org.Name,
                    CreatedAt = org.CreatedAt,
                    UpdatedAt = org.UpdatedAt
                }).ToList();

                return new PagedResponse<OrganizationResponseDto, Guid>
                {
                    Data = mappedData,
                    PageNumber = pagedEntities.PageNumber,
                    PageSize = pagedEntities.PageSize,
                    TotalItems = pagedEntities.TotalItems,
                    TotalPages = pagedEntities.TotalPages,
                    LastSeenId = pagedEntities.LastSeenId
                };
            }) ?? new PagedResponse<OrganizationResponseDto, Guid>();
        }

        public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto dto, CancellationToken cancellationToken = default)
        {
            var organization = new Organization
            {
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await organizationRepository.AddAsync(organization, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                ClearCache();

                return new OrganizationResponseDto
                {
                    Id = organization.Id.ToString(),
                    Name = organization.Name,
                    CreatedAt = organization.CreatedAt,
                    UpdatedAt = organization.UpdatedAt
                };
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdateOrganizationAsync(Guid id, OrganizationUpdateDto dto, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var organization = await organizationRepository.GetByIdAsync(id, cancellationToken);
                if (organization == null) return;

                organization.Name = dto.Name;
                organization.UpdatedAt = DateTime.UtcNow;

                organizationRepository.Update(organization);
                await unitOfWork.SaveChangesAsync(cancellationToken);
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

        public async Task DeleteOrganizationAsync(Guid id, CancellationToken cancellationToken = default)
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
            cache.Remove(AllOrgsCacheKey); 
        }
    }
}
