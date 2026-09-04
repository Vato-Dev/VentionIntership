using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Api.GraphQl
{
    public class Query
    {
            [UseFiltering]
            [UseSorting]
            public IQueryable<FileModel> GetFiles([Service] AppDbContext context, Guid orgId)
            {
                return context.Files.Where(f => f.OrganisationId == orgId);
            }
    
            public async Task<FileModel?> GetFile([Service] AppDbContext context, Guid id)
            {
                return await context.Files.FirstOrDefaultAsync(f => f.Id == id);
            }
    }
}
