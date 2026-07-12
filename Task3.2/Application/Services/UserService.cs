using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;

namespace Application.Services
{
  public sealed class UserService(IBaseRepository<User> userRepository, IUnitOfWork unitOfWork) : IUserService
    {
        public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default) 
            => userRepository.GetByIdAsync(id, cancellationToken);

        public Task<PagedResponse<User>> GetAllUsersAsync(int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default) 
            => userRepository.GetAllAsync(cancellationToken, keySetId, page, pageSize);

        public async Task CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await userRepository.AddAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var user = await userRepository.GetByIdAsync(id, cancellationToken);
                if (user != null)
                {
                    userRepository.Delete(user);
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