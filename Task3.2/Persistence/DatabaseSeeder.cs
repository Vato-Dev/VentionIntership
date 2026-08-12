using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Positions
        if (!await context.Positions.AnyAsync())
        {
            context.Positions.AddRange(
                new Position
                {
                    Title = "Junior Backend Developer",
                    Description = "Responsible for building core server-side logic using .NET."
                },
                new Position
                {
                    Title = "Senior Fullstack Engineer",
                    Description = "Leads architecture and handles both frontend and backend systems."
                },
                new Position
                {
                    Title = "Project Manager",
                    Description = "Coordinates timelines, team deliveries, and client goals."
                }
            );
            await context.SaveChangesAsync();
        }

        // Organizations
        if (!await context.Organizations.AnyAsync())
        {
            context.Organizations.AddRange(
                new Organization
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "TechVanguard Solutions",
                    StreetAddress = "128 Innovation Way, Suite 400",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Organization
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Apex Global Systems",
                    StreetAddress = "45 Parallel Avenue",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
        }

        // Users
        if (!await context.Users.AnyAsync())
        {
            var juniorPosition = await context.Positions.FirstAsync(p => p.Title == "Junior Backend Developer");
            var seniorPosition = await context.Positions.FirstAsync(p => p.Title == "Senior Fullstack Engineer");

            var user1Id = Guid.NewGuid().ToString();
            var user2Id = Guid.NewGuid().ToString();

            context.Users.AddRange(
                new User
                {
                    Id = user1Id,
                    Name = "vlad_osman",
                    Email = "vladimer.osmanovi@example.com",
                    Role = "User",
                    PositionId = juniorPosition.Id,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = user2Id,
                    Name = "gia_ghariba",
                    Email = "gia.gharibashvili@example.com",
                    Role = "Admin",
                    PositionId = seniorPosition.Id,
                    CreatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();

            // Memberships
            var org1 = await context.Organizations.FirstAsync(o => o.Name == "TechVanguard Solutions");

            context.Memberships.AddRange( 
                new Membership
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user1Id,
                    OrganizationId = org1.Id,
                    Role = "Member",
                    CreatedAt = DateTime.UtcNow
                },
                new Membership
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user2Id,
                    OrganizationId = org1.Id,
                    Role = "Owner",
                    CreatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();

            // Sessions
            context.Sessions.AddRange(
                new Session
                {
                    AccessToken = Guid.NewGuid().ToString("N"),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsActive = true,
                    UserId = user1Id
                },
                new Session
                {
                    AccessToken = Guid.NewGuid().ToString("N"),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsActive = true,
                    UserId = user2Id
                }
            );
            await context.SaveChangesAsync();
        }
    }
}