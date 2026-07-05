
using System.Data; 
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;

namespace Application.Services
{
    public sealed class SessionService(IBaseRepository<Session> sessionRepository, IUnitOfWork unitOfWork) : ISessionService
    {
        public Task<Session?> GetSessionByIdAsync(int id) => sessionRepository.GetByIdAsync(id);

        public Task<PagedResponse<Session>> GetAllSessionsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10) 
            => sessionRepository.GetAllAsync(keySetId, page, pageSize);
        public async Task CreateSessionAsync(Session session)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                await sessionRepository.AddAsync(session);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateSessionAsync(Session session)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                sessionRepository.Update(session);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteSessionAsync(int id)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var session = await sessionRepository.GetByIdAsync(id);
                if (session != null)
                {
                    sessionRepository.Delete(session);
                    await unitOfWork.SaveChangesAsync();
                }
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}