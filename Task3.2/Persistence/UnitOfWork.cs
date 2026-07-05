using System.Data;
using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence
{
        public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
        {
            private IDbContextTransaction? _currentTransaction;

            public Task<int> SaveChangesAsync()
            {
                return context.SaveChangesAsync();
            }

            public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
            {
                if (_currentTransaction != null)
                {
                    return;
                }

                _currentTransaction = await context.Database.BeginTransactionAsync(isolationLevel);
            }

            public async Task CommitTransactionAsync()
            {
                try
                {
                    if (_currentTransaction != null)
                    {
                        await _currentTransaction.CommitAsync();
                    }
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
                finally
                {
                    DisposeTransaction();
                }
            }

            public async Task RollbackTransactionAsync()
            {
                try
                {
                    if (_currentTransaction != null)
                    {
                        await _currentTransaction.RollbackAsync();
                    }
                }
                finally
                {
                    DisposeTransaction();
                }
            }

            public void Dispose()
            {
                DisposeTransaction();
                context.Dispose();
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
