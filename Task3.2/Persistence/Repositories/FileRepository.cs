using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Persistence.Repositories
{
    public sealed class FileRepository(AppDbContext context) : IFileRepository
    {
        public async Task AddAsync(FileModel file, CancellationToken ct = default)
        {
            await context.Files.AddAsync(file, ct);
            await context.SaveChangesAsync(ct);
        }
        
        
        public Task<FileModel?> GetByHashAsync(string checksum, Guid organisationId, CancellationToken ct = default) =>
            context.Files.SingleOrDefaultAsync(f => f.Checksum == checksum && f.OrganisationId == organisationId, ct);
        
        public async Task<PagedResponse<FileModel, Guid>> GetByOrganizationIdAsync(Guid organisationId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = context.Files
                .AsNoTracking()
                .Where(f => f.OrganisationId == organisationId)
                .OrderByDescending(f => f.CreatedAt);

            var totalItems = await query.CountAsync(ct);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var skipCount = (page - 1) * pageSize;

            var data = await query.Skip(skipCount).Take(pageSize).ToListAsync(ct);

            return new PagedResponse<FileModel, Guid>
            {
                Data = data,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public Task<FileModel?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            context.Files.SingleOrDefaultAsync(f => f.Id == id, ct);

        public async Task DeleteAsync(FileModel file, CancellationToken ct = default)
        {
            context.Files.Remove(file);
            await context.SaveChangesAsync(ct);
        }
    }
   
}
