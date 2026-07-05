using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
        
    }
}
