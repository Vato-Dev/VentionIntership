using Application.Abstractions;
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
        
    }
   
}
