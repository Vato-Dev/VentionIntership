using Application.Abstractions;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T>
        where T : class
    {

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Set<T>().FindAsync(id, cancellationToken);
        }

        public async Task<PagedResponse<T>> GetAllAsync( CancellationToken cancellationToken,int? keySetId = null, int? page = 1, int? pageSize = 10)
        {
            var baseQuery = context.Set<T>().AsNoTracking();
            int size = pageSize ?? 10;
            int p = page ?? 1;

            var query = baseQuery.OrderBy(x => EF.Property<int>(x, "Id"));

            if (keySetId.HasValue && keySetId.Value > 0)
            {
                query = (IOrderedQueryable<T>)query.Where(x => EF.Property<int>(x, "Id") > keySetId.Value);
                var data = await query.Take(size).ToListAsync(cancellationToken);

                var lastItem = data.LastOrDefault();
                int? lastId = lastItem != null ? ((dynamic)lastItem).Id : null;
                return new PagedResponse<T>
                {
                    Data = data,
                    PageSize = size,
                    LastSeenId = lastId
                };
            }
            else
            {
       //otherwise default offset 
                int totalItems = await baseQuery.CountAsync(cancellationToken); 
        
                int skipCount = (p - 1) * size;
                var data = await query.Skip(skipCount).Take(size).ToListAsync(cancellationToken);
        
                int totalPages = (int)Math.Ceiling((double)totalItems / size);

                return new PagedResponse<T>
                {
                    Data = data,
                    PageNumber = p,
                    PageSize = size,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                };
            }
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
            context.Set<T>().Remove(entity);
        }
    }
}
