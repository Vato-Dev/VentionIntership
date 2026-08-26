
using System.Data;
using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IBaseRepository<T, TKey> 
        where T : DomainEntity<TKey>
        where TKey : struct,IComparable<TKey>
    {
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken);
        Task<PagedResponse<T, TKey>> GetAllAsync(CancellationToken cancellationToken, TKey? keySetId = default, int? page = 1, int? pageSize = 10);
        Task AddAsync(T entity, CancellationToken cancellationToken);
        void Update(T entity);
        void Delete(T entity);
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
