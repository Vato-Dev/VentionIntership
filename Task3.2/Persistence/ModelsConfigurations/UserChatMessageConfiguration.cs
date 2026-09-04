using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public class UserChatMessageConfiguration : IEntityTypeConfiguration<UserChatMessage>
    {
        public void Configure(EntityTypeBuilder<UserChatMessage> builder)
        {
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(10000);

            builder.Property(m => m.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(m => m.IsRead)
                .IsRequired();

            builder.HasIndex(m => m.ChatId);
            builder.HasIndex(m => m.SenderId);
            builder.HasIndex(m => m.CreatedAt);
            builder.HasIndex(m => new { m.ChatId, m.CreatedAt });
        }
    }
}
