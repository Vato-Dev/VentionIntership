
using System.Data;

namespace Application.Abstractions
{
    public interface IBaseRepository<TModel> where TModel : class
    {
        Task<TModel?> GetByIdAsync(int id);
        Task<IEnumerable<TModel>> GetAllAsync();
        Task AddAsync(TModel entity);
        void Update(TModel entity);
        void Delete(TModel entity);
    }
    public interface IUnitOfWork 
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
    //todo: refactor later
}
