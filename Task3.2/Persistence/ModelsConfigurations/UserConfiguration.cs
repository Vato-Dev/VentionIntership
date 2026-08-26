using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(256)
                .IsRequired();
            
            builder.Property(u => u.Id)
                .HasDefaultValueSql("uuidv7()")
                .ValueGeneratedOnAdd();
            
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}