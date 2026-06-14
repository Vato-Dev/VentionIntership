using LibraryManager.Application.Interfaces;
using LibraryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManger.Persistence.Repositories
{
    public class BorrowingRepository : BaseRepository<BookBorrowing>, IBorrowingRepository
    {
        public BorrowingRepository(AppDbContext context) : base(context)
        {
            
        }
        
        public Task<BookBorrowing?> GetBorrowingWithBookByIdAsync(int borrowingId , CancellationToken ct)
         => _dbSet.Include(x=>x.Book)
                .FirstOrDefaultAsync(x=>x.Id == borrowingId, ct);
        
        
        public Task<bool> IsBorrowed(int bookId, int readerId, CancellationToken ct)
            => _dbSet.AnyAsync(x => x.BookId == bookId && 
                                    x.ReaderId == readerId && 
                                    !x.IsReturned, ct);
    }
}
