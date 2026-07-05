
using System.Data;
using Application.DTOs;

namespace Application.Abstractions
{
    public interface IBaseRepository<TModel> where TModel : class
    {
        Task<TModel?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<PagedResponse<TModel>> GetAllAsync(CancellationToken cancellationToken,int? keySetId = null, int? page = 1, int? pageSize = 10 );
        Task AddAsync(TModel entity, CancellationToken cancellationToken);
        void Update(TModel entity);
        void Delete(TModel entity);
    }
    public interface IUnitOfWork 
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }
    //todo: refactor later
}
