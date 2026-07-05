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
                    new Position { Title = "Junior Backend Developer", Description = "Responsible for building core server-side logic using .NET." },
                    new Position { Title = "Senior Fullstack Engineer", Description = "Leads architecture and handles both frontend and backend systems." },
                    new Position { Title = "Project Manager", Description = "Coordinates timelines, team deliveries, and client goals." }
                );
                context.SaveChanges(); 
            }

            if (!context.Organizations.Any())
            {
                context.Organizations.AddRange(
                    new Organization { Name = "TechVanguard Solutions", StreetAddress = "128 Innovation Way, Suite 400" },
                    new Organization { Name = "Apex Global Systems", StreetAddress = "45 Parallel Avenue" }
                );
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User 
                    { 
                        Username = "vlad_osman", 
                        Email = "vladimer.osmanovi@example.com", 
                        PositionId = 1, 
                        OrganizationId = 1, 
                        CreatedAt = DateTime.UtcNow 
                    },
                    new User 
                    { 
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