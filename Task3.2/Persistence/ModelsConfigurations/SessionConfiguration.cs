using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.CreatedAt).HasConversion(c => c, 
            c => DateTime.SpecifyKind(c, DateTimeKind.Utc))
                .HasColumnType("timestamp with time zone");

            builder.Property(s => s.ExpiresAt).HasConversion(c => c, 
            c => DateTime.SpecifyKind(c, DateTimeKind.Utc))
                .HasColumnType("timestamp with time zone");
            

            builder.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}