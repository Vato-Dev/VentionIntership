using Application.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class UserRepository(AppDbContext context) : BaseRepository<User,Guid>(context),IUserRepository
    {
        //todo: make an simple CQRS to differentiate reading from insert/update/delete and optimize code and sql generated behind it
        public Task<User?> GetByEmailNoTrackingAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.ToLower();
         return  DbSet.AsNoTracking().SingleOrDefaultAsync(u=>normalizedEmail == u.Email, ct);
        }
    }
}
