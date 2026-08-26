using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class DatabaseSeeder
{
    public async static Task SeedAsync(AppDbContext context)
    {
        if (!await context.Organizations.AnyAsync())
        {
            context.Organizations.AddRange(
                new Organization
                {
                    Name = "TechVanguard Solutions",
                    StreetAddress = "128 Innovation Way, Suite 400",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Organization
                {
                    Name = "Apex Global Systems",
                    StreetAddress = "45 Parallel Avenue",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            DateTime nowUtc = DateTime.UtcNow;

            var user1 = new User
            {
                Name = "vlad_osman",
                Email = "vladimer.osmanovi@example.com",
                Role = "User",
                PasswordHash = "someHashInFuture", 
                CreatedAt = nowUtc
            };

            var user2 = new User
            {
                Name = "gia_ghariba",
                Email = "gia.gharibashvili@example.com",
                Role = "Admin",
                PasswordHash = "someHashInFuture", 
                CreatedAt = nowUtc
            };

            context.Users.AddRange(user1, user2);


            await context.SaveChangesAsync();

            var org1 = await context.Organizations.FirstAsync(o => o.Name == "TechVanguard Solutions");

            context.Memberships.AddRange( 
                new Membership
                {
                    UserId = user1.Id, 
                    OrganizationId = org1.Id,
                    Role = "Member",
                    CreatedAt = nowUtc
                },
                new Membership
                {
                    UserId = user2.Id, 
                    OrganizationId = org1.Id,
                    Role = "Owner",
                    CreatedAt = nowUtc
                }
            );

            context.Sessions.AddRange(
                new Session
                {
                    AccessToken = Guid.NewGuid().ToString("N"), 
                    CreatedAt = nowUtc,
                    ExpiresAt = nowUtc.AddDays(7),
                    IsActive = true,
                    UserId = user1.Id
                },
                new Session
                {
                    AccessToken = Guid.NewGuid().ToString("N"),
                    CreatedAt = nowUtc,
                    ExpiresAt = nowUtc.AddDays(7),
                    IsActive = true,
                    UserId = user2.Id
                }
            );

            // Финальное сохранение всех связей
            await context.SaveChangesAsync();
        }
    }
}
