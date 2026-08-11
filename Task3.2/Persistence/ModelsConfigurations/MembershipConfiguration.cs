using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(m => m.Id);
            
            builder.HasOne(m=>m.User)
                .WithMany(m=>m.Memberships)
                .HasForeignKey(m=>m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(m=>m.Organization)
                .WithMany(m=>m.Memberships)
                .HasForeignKey(m=>m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(x => x.CreatedAt)
                .HasConversion(
                c => c, 
                c => c.HasValue ? DateTime.SpecifyKind(c.Value, DateTimeKind.Utc) : null)
                .HasColumnType("timestamp with time zone");      
        }
    }
}
