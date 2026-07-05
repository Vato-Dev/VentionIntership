
using System.Data;
using Application.DTOs;

namespace Application.Abstractions
{
    public interface IBaseRepository<TModel> where TModel : class
    {
        Task<TModel?> GetByIdAsync(int id);
        Task<PagedResponse<TModel>> GetAllAsync(int? keySetId = null, int? page = 1, int? pageSize = 10);
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
