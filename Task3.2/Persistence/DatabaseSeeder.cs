using System.Security.Cryptography;
using System.Text;
using Domain.Extensions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class DatabaseSeeder
{
    public async static Task SeedAsync(AppDbContext context)
    {
        DateTime nowUtc = DateTime.UtcNow;

        // ==========================================
        // 1. Сидинг организаций (Идемпотентный)
        // ==========================================
        var org1Name = "TechVanguard Solutions";
        var org2Name = "Apex Global Systems";

        if (!await context.Organizations.AnyAsync(o => o.Name == org1Name))
        {
            context.Organizations.Add(new Organization
            {
                Name = org1Name,
                StreetAddress = "128 Innovation Way, Suite 400",
                CreatedAt = nowUtc,
                UpdatedAt = nowUtc
            });
        }

        if (!await context.Organizations.AnyAsync(o => o.Name == org2Name))
        {
            context.Organizations.Add(new Organization
            {
                Name = org2Name,
                StreetAddress = "45 Parallel Avenue",
                CreatedAt = nowUtc,
                UpdatedAt = nowUtc
            });
        }

        await context.SaveChangesAsync();

        var targetOrg = await context.Organizations.FirstAsync(o => o.Name == org1Name);



        var user1Email = "vladimer.osmanovi@example.com";
        var user2Email = "gia.gharibashvili@example.com";

        User? user1 = await context.Users.FirstOrDefaultAsync(u => u.Email == user1Email);
        User? user2 = await context.Users.FirstOrDefaultAsync(u => u.Email == user2Email);

        if (user1 == null)
        {
            user1 = new User
            {
                Name = "vlad_osman",
                Email = user1Email,
                Role = "USER",
                PasswordHash = HashPasswordForSeeder("test123"), 
                CreatedAt = nowUtc
            };
            context.Users.Add(user1);
            await context.SaveChangesAsync(); 

            if (!await context.Memberships.AnyAsync(m => m.UserId == user1.Id && m.OrganizationId == targetOrg.Id))
            {
                context.Memberships.Add(new Membership
                {
                    UserId = user1.Id, 
                    OrganizationId = targetOrg.Id,
                    Role = "MEMBER",
                    CreatedAt = nowUtc
                });
            }

            context.Sessions.Add(new Session
            {
                AccessToken = Guid.NewGuid().ToString("N"), 
                CreatedAt = nowUtc,
                ExpiresAt = nowUtc.AddDays(7),
                IsActive = true,
                UserId = user1.Id
            });
        }

        if (user2 == null)
        {
            user2 = new User
            {
                Name = "gia_ghariba",
                Email = user2Email,
                Role = "ADMIN",
                PasswordHash = HashPasswordForSeeder("test123"),
                CreatedAt = nowUtc
            };
            context.Users.Add(user2);
            await context.SaveChangesAsync(); 

            if (!await context.Memberships.AnyAsync(m => m.UserId == user2.Id && m.OrganizationId == targetOrg.Id))
            {
                context.Memberships.Add(new Membership
                {
                    UserId = user2.Id, 
                    OrganizationId = targetOrg.Id,
                    Role = "OWNER",
                    CreatedAt = nowUtc
                });
            }

            context.Sessions.Add(new Session
            {
                AccessToken = Guid.NewGuid().ToString("N"),
                CreatedAt = nowUtc,
                ExpiresAt = nowUtc.AddDays(7),
                IsActive = true,
                UserId = user2.Id
            });
        }
        
        await context.SaveChangesAsync();
    }
    
    private static string HashPasswordForSeeder(string password)
    {
        var pepper = "PEPPER".FromEnvRequired(); 
        var pepperBytes = Encoding.UTF8.GetBytes(pepper);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var hmac = new HMACSHA256(pepperBytes);
        var hash = hmac.ComputeHash(passwordBytes);
        var base64Password = Convert.ToBase64String(hash);

        return BCrypt.Net.BCrypt.HashPassword(base64Password, 11);
    }
}