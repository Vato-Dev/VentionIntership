
using Application.Abstractions;
using Domain.Models;

namespace Application.Services
{
    public sealed class UserService(IBaseRepository<User> userRepository, IUnitOfWork unitOfWork) : IUserService
    {
        public Task<User?> GetUserByIdAsync(int id) => userRepository.GetByIdAsync(id);

        public Task<IEnumerable<User>> GetAllUsersAsync() => userRepository.GetAllAsync();

        public async Task CreateUserAsync(User user)
        {
            await userRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user != null)
            {
                userRepository.Delete(user);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
