
using System.Data; 
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;

namespace Application.Services
{
  public sealed class SessionService(IBaseRepository<Session> sessionRepository, IUnitOfWork unitOfWork) : ISessionService
    {
        public Task<Session?> GetSessionByIdAsync(int id, CancellationToken cancellationToken = default) 
            => sessionRepository.GetByIdAsync(id, cancellationToken);

        public Task<PagedResponse<Session>> GetAllSessionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default) 
            => sessionRepository.GetAllAsync(cancellationToken, keySetId, page, pageSize);

        public async Task CreateSessionAsync(Session session, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await sessionRepository.AddAsync(session, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdateSessionAsync(Session session, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                sessionRepository.Update(session);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task DeleteSessionAsync(int id, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var session = await sessionRepository.GetByIdAsync(id, cancellationToken);
                if (session != null)
                {
                    sessionRepository.Delete(session);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }
    }
}