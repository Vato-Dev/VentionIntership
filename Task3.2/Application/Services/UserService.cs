using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;

namespace Application.Services
{
    public sealed class UserService(IBaseRepository<User> userRepository, IUnitOfWork unitOfWork) : IUserService
    {
        public Task<User?> GetUserByIdAsync(int id) => userRepository.GetByIdAsync(id);

        public Task<PagedResponse<User>> GetAllUsersAsync(int? keySetId = null, int? page = 1, int? pageSize = 10) 
            => userRepository.GetAllAsync(keySetId, page, pageSize);
        public async Task CreateUserAsync(User user)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                await userRepository.AddAsync(user);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userRepository.GetByIdAsync(id);
                if (user != null)
                {
                    userRepository.Delete(user);
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