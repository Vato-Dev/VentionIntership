using Application.Abstractions;
using Domain.Models;

namespace Application.DTOs
{
    public interface IUserRepository : IBaseRepository<User,Guid>
    {
        Task<User?> GetByEmailNoTrackingAsync(string email, CancellationToken ct = default);
    }
}
