using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public class FileModelConfiguration : IEntityTypeConfiguration<FileModel>
    {
        public void Configure(EntityTypeBuilder<FileModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id)
                .HasDefaultValueSql("uuidv7()")
                .ValueGeneratedOnAdd();
            
            builder.Property(f => f.Filename).IsRequired().HasMaxLength(1024);
            builder.Property(f => f.ContentType).IsRequired().HasMaxLength(255);
            builder.Property(f => f.Checksum).IsRequired().HasMaxLength(64);
            builder.Property(f => f.StorageKey).IsRequired().HasMaxLength(1024);
            
            builder.HasIndex(f => new { f.OrganisationId, f.Checksum })
                .IsUnique();
            
            builder.HasIndex(f => f.OwnerId); // i added this in case if i'll need to get "all my files" logic
        }
    }
}
