using System.Data;
using System.Text.Json;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services
{
    public sealed class OrganizationService(
        IBaseRepository<Organization, Guid> organizationRepository,
        IUnitOfWork unitOfWork,
        IDistributedCache cache) : IOrganizationService
    {
        private const string CacheKeyPrefix = "org_";
        private const string AllOrgsCacheKey = "all_orgs_paged_";
        private const string ListVersionKey = "org_list_version";
        
        private async Task<int> GetListVersionAsync(CancellationToken ct)
        {
            var raw = await cache.GetStringAsync(ListVersionKey, ct);
            return int.TryParse(raw, out var v) ? v : 0;
        }

        private async Task BumpListVersionAsync(CancellationToken ct)
        {
            var current = await GetListVersionAsync(ct);
          
            await cache.SetStringAsync(ListVersionKey, (current + 1).ToString(),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) }, ct);
        }
        
        private const string NullSentinel = "__NULL__";

        public async Task<OrganizationResponseDto?> GetOrganizationByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (cached != null)
            {
                return cached == NullSentinel ? null : JsonSerializer.Deserialize<OrganizationResponseDto>(cached);
            }

            var org = await organizationRepository.GetByIdAsync(id, cancellationToken);
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

            if (org == null)
            {
                await cache.SetStringAsync(cacheKey, NullSentinel, options, cancellationToken);
                return null;
            }

            var dto = new OrganizationResponseDto
            {
                Id = org.Id.ToString(),
                Name = org.Name,
                CreatedAt = org.CreatedAt,
                UpdatedAt = org.UpdatedAt
            };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options, cancellationToken);
            return dto;
        }

        public async Task<PagedResponse<OrganizationResponseDto, Guid>> GetAllOrganizationsAsync(
            Guid? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            var version = await GetListVersionAsync(cancellationToken);
            var cacheKey = $"{AllOrgsCacheKey}v{version}_{keySetId}_{page}_{pageSize}";

            var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<PagedResponse<OrganizationResponseDto, Guid>>(cached)!;
            }

            var pagedEntities = await organizationRepository.GetAllAsync(cancellationToken, keySetId, page, pageSize);

            var mappedData = pagedEntities.Data.Select(org => new OrganizationResponseDto
            {
                Id = org.Id.ToString(),
                Name = org.Name,
                CreatedAt = org.CreatedAt,
                UpdatedAt = org.UpdatedAt
            }).ToList();

            var result = new PagedResponse<OrganizationResponseDto, Guid>
            {
                Data = mappedData,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize,
                TotalItems = pagedEntities.TotalItems,
                TotalPages = pagedEntities.TotalPages,
                LastSeenId = pagedEntities.LastSeenId
            };

            // IMemoryCache had SlidingExpiration too - IDistributedCache supports it the same
            // way, both can be set together.
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            }, cancellationToken);

            return result;
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
                await BumpListVersionAsync(cancellationToken);

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

                await cache.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);
                await BumpListVersionAsync(cancellationToken);
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

                await cache.RemoveAsync($"{CacheKeyPrefix}{id}", cancellationToken);
                await BumpListVersionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }
    }
}