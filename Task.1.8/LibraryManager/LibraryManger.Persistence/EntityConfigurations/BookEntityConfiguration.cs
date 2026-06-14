using LibraryManager.Domain.Models;
using LibraryManager.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManger.Persistence.EntityConfigurations
{
    public sealed class BookEntityConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Title).HasMaxLength(100);
            builder.Property(b => b.Isbn).HasMaxLength(13);
            builder.Property(b => b.AuthorName).HasMaxLength(100);
            
            builder.HasOne(b => b.Catalogue)
                .WithMany(x=>x.BookCopies)
                .HasForeignKey(x => x.CatalogueId);
            
            builder.Navigation(b => b.Catalogue);
            
            builder.Property(b=>b.Condition).HasConversion<string>();
            builder.Property(b=>b.Isbn).HasConversion(x=>x.Value , v=> Isbn.Create(v)).HasMaxLength(13);

            builder.HasIndex(x=>x.Isbn).IsUnique();
            builder.HasIndex(x=>x.Title);
        }
    }
    
    public sealed class ReaderEntityConfiguration : IEntityTypeConfiguration<Reader>
    {
        public void Configure(EntityTypeBuilder<Reader> builder)
        {
            builder.Property(b => b.Name).HasMaxLength(100);
            builder.Property(b => b.PersonalNumber).HasMaxLength(20); //11 but different country format
            builder.Property(b => b.PhoneNumber).HasMaxLength(20);
            builder.Property(b => b.EmailAddress).HasMaxLength(40);
                
            builder.Property(b =>b.RegisteredAt)
                .HasColumnType("datetime2");

            builder.Navigation(b => b.BookBorrowings)
                .HasField("_bookBorrowings")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.HasIndex(x=>x.EmailAddress).IsUnique();
            builder.Property(b=>b.Status).HasConversion<string>();
        }
    }
    

    public sealed class FineEntityConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.Property(x=>x.GaveAt).HasColumnType("datetime2");
            builder.Property(x=>x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x=>x.Description).HasMaxLength(500);
        }
    }
    public sealed class CatalogueConfiguration: IEntityTypeConfiguration<Catalogue>
    {
        public void Configure(EntityTypeBuilder<Catalogue> builder)
        {
            builder.Property(b=>b.Name).HasMaxLength(100).IsRequired();
            
            builder.HasOne<Catalogue>().WithMany()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            
            builder.Navigation(b=>b.BookCopies)
                .HasField("_bookCopies")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.HasIndex(x=>x.Name).IsUnique();
            // if i will need to call parents props i'll add navigation prop
        }
    }

    public sealed class BookBorrowingConfiguration : IEntityTypeConfiguration<BookBorrowing>
    {
        public void Configure(EntityTypeBuilder<BookBorrowing> builder)
        {
            builder.ToTable("BookBorrowings");
            builder.Property(x=>x.BorrowedAt).HasColumnType("datetime2");
            builder.Property(x=>x.ReturnedAt).HasColumnType("datetime2");

            builder.HasOne(b=>b.Book).WithMany()
                .HasForeignKey(b=>b.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(b=>b.Reader).WithMany(x=>x.BookBorrowings)
                .HasForeignKey(b=>b.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            
            builder.Navigation(b=>b.Fines).HasField("_fines")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.HasIndex( x=> new {x.BookId,x.ReaderId});
        }
    }
}
