using LibraryManager.Application.Interfaces;
using LibraryManager.Application.Services;
using LibraryManager.Domain.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace LibraryManger.Persistence.Querries
{
    public sealed class BookQuerries(AppDbContext context) : IBookQuerries
    {
        /*  public async Task<BookInfoDto?> GetByIdAsync(int id, CancellationToken ct) =>
          await context.Books.AsNoTracking()
                .Where(x => x.Id == id)
                .Join(context.Catalogues,x=>x.CatalogueId, inner => inner.Id, (book, catalogue) => new BookInfoDto(
                    book.Title,
                    book.Isbn,
                    book.AuthorName,
                    book.PublishYear,
                    catalogue.Name,
                    book.Condition
                    )).FirstOrDefaultAsync(ct);*/ //UnOptimized shit not to do this since DDD does not matter in queries + it has no backups and smart logic EF core gives us with include use Dapper or projection directly

        public async Task<BookInfoDto?> GetByIdAsync(int id, CancellationToken ct) => //better version of code above , but works only if i've 
           await context.Books.Include(x=>x.Catalogue).AsNoTracking().Where(x => x.Id == id).ProjectToType<BookInfoDto>().FirstOrDefaultAsync(ct);
    }
}
