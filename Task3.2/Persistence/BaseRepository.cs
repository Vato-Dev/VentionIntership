using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class BaseRepository<T, TKey>(AppDbContext context) : IBaseRepository<T, TKey>
        where T : DomainEntity<TKey>
        where TKey : struct, IComparable<TKey>
    {
        
        protected DbSet<T> DbSet = null!;
        public async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await context.Set<T>().FindAsync([id], cancellationToken);
        }

        public async Task<PagedResponse<T, TKey>> GetAllAsync(
            CancellationToken cancellationToken, 
            TKey? keySetId = null,
            int? page = 1, 
            int? pageSize = 10)
        {
            var size = pageSize ?? 10;
            var currentBufferPage = page ?? 1;

            IQueryable<T> query = context.Set<T>().AsNoTracking().OrderBy(x => x.Id);

            if (keySetId.HasValue && !keySetId.Value.Equals(default(TKey)))
            {
                query = query.Where(x => EF.Property<TKey>(x, "Id").CompareTo(keySetId.Value) > 0);

                var data = await query.Take(size).ToListAsync(cancellationToken);
                var lastItem = data.LastOrDefault();
                TKey? lastId = lastItem != null ? lastItem.Id : default;

                return new PagedResponse<T, TKey> { Data = data, PageSize = size, LastSeenId = lastId ?? default};
            }
            
            int totalItems = await context.Set<T>().AsNoTracking().CountAsync(cancellationToken); 
            int skipCount = (currentBufferPage - 1) * size;
            
            var offsetData = await query.Skip(skipCount).Take(size).ToListAsync(cancellationToken);
            int totalPages = (int)Math.Ceiling((double)totalItems / size);

            return new PagedResponse<T, TKey>
            {
                Data = offsetData,
                PageNumber = currentBufferPage,
                PageSize = size,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await context.Set<T>().AddAsync(entity, cancellationToken); 
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public void Delete(T entity) 
        {
            var entry = context.Entry(entity);
            
            if (entry.State == EntityState.Detached)
            {
                context.Set<T>().Attach(entity);
            }
            context.Set<T>().Remove(entity);
        }
    }
}
