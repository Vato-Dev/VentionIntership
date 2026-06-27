using System;
using System.Linq;
using Domain.Models;

namespace Persistence
{
    public static class DatabaseSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Positions.Any())
            {
                context.Positions.AddRange(
                    new Position { Id = 1, Title = "Junior Backend Developer", Description = "Responsible for building core server-side logic using .NET." },
                    new Position { Id = 2, Title = "Senior Fullstack Engineer", Description = "Leads architecture and handles both frontend and backend systems." },
                    new Position { Id = 3, Title = "Project Manager", Description = "Coordinates timelines, team deliveries, and client goals." }
                );
                context.SaveChanges(); 
            }

            if (!context.Organizations.Any())
            {
                context.Organizations.AddRange(
                    new Organization { Id = 1, Name = "TechVanguard Solutions", StreetAddress = "128 Innovation Way, Suite 400" },
                    new Organization { Id = 2, Name = "Apex Global Systems", StreetAddress = "45 Parallel Avenue" }
                );
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User 
                    { 
                        Id = 1, 
                        Username = "vlad_osman", 
                        Email = "vladimer.osmanovi@example.com", 
                        PositionId = 1, 
                        OrganizationId = 1, 
                        CreatedAt = DateTime.UtcNow 
                    },
                    new User 
                    { 
                        Id = 2, 
                        Username = "gia_ghariba", 
                        Email = "gia.gharibashvili@example.com", 
                        PositionId = 2, 
                        OrganizationId = 1, 
                        CreatedAt = DateTime.UtcNow 
                    }
                );
                context.SaveChanges();
            }

            if (!context.Sessions.Any())
            {
                context.Sessions.Add(
                    new Session 
                    { 
                        Id = 1, 
                        UserId = 1, 
                        IsActive = true, 
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(1)
                    }
                );
                context.SaveChanges();
            }
        }
    }
}