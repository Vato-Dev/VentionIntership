using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(u => u.Id)
                .HasDefaultValueSql("uuidv7()")
                .ValueGeneratedOnAdd();
            
            builder.Property(o => o.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(o => o.StreetAddress)
                .HasMaxLength(500)
                .IsRequired();
            
            builder.Property(c=>c.CreatedAt)
                .HasConversion(c=>c, 
                c=> c.HasValue ? DateTime.SpecifyKind(c.Value, DateTimeKind.Utc):null)
                .HasColumnType("timestamp with time zone");      
            
            builder.Property(c=>c.UpdatedAt)
                .HasConversion(c=>c, 
                c=> c.HasValue ? DateTime.SpecifyKind(c.Value, DateTimeKind.Utc):null)
                .HasColumnType("timestamp with time zone");      

        }
    }
}