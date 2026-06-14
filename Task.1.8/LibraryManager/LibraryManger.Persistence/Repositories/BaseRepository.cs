using LibraryManager.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManger.Persistence
{
    public class BaseRepository<TModel> : IBaseRepository<TModel> where TModel : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TModel> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TModel>();
        }

        public async Task<TModel?> GetByIdAsync(int id, CancellationToken cancellationToken) => await _dbSet.FindAsync(keyValues: [id], cancellationToken: cancellationToken);
        public void Add(TModel entity) =>  _dbSet.AddAsync(entity);
        public void Update(TModel entity) => _dbSet.Update(entity);
        public void Delete(TModel entity) => _dbSet.Remove(entity);
    }
}
