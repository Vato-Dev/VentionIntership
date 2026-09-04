using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.ModelsConfigurations
{
    public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
    {
        public void Configure(EntityTypeBuilder<UserChat> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.UserId1)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.UserId2)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.LastMessage)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.LastMessageAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.UnreadCount)
                .IsRequired();

            builder.HasIndex(c => c.UserId1);
            builder.HasIndex(c => c.UserId2);
            builder.HasIndex(c => c.LastMessageAt);
        }
    }
}
