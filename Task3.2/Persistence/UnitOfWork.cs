using System.Data;
using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence
{
    public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _currentTransaction;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return;

            _currentTransaction = await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                DisposeTransaction();
            }
        }

        private void DisposeTransaction()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
