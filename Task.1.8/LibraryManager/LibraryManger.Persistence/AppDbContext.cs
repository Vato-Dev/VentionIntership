using LibraryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManger.Persistence.EntityConfigurations;

namespace LibraryManger.Persistence
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options ) : DbContext(options)
    {
        public DbSet<Book> Books {  get; set; }
        public DbSet<BookBorrowing> BookBorrowing { get; set; }
        public DbSet<Catalogue> Catalogues { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Fine> Fines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogueConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
