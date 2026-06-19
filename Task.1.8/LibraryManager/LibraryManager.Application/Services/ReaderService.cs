using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Application.Interfaces;
using LibraryManager.Domain.Models;
using Mapster;

namespace LibraryManager.Application.Services
{
  public sealed class ReaderService(
        IBaseRepository<Reader> readerRepository,
        IUnitOfWork unitOfWork) : IReaderService
    {

        public async Task<int> CreateReader(AddReaderRequest request, CancellationToken ct)
        {
           var reader = request.Adapt<Reader>();
           reader.RegisteredAt = DateTime.UtcNow;
            readerRepository.Add(reader);
            await unitOfWork.SaveChangesAsync(ct);
            return reader.Id;
        }

        public async Task UpdateReaderProfile(UpdateReaderRequest request, CancellationToken ct)
        {
            var reader = await readerRepository.GetByIdAsync(request.Id, ct);
            if (reader == null) throw new Exception("Reader not found");

            if (reader.Status != ReaderState.Active)
             throw new Exception("Reader is not active");
            
            request.Adapt(reader);
            
            
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteReader(int id, CancellationToken ct)
        {
            var reader = await readerRepository.GetByIdAsync(id, ct);
            if (reader == null) throw new Exception("Reader not found");

            var hasActiveLoans = reader.BookBorrowings.Any(b => !b.IsReturned);
            if (hasActiveLoans)
                throw new Exception("Cannot delete reader while they have unreturned books.");
            
            var unpaidFinesAmount = reader.BookBorrowings
                .SelectMany(b => b.Fines) 
                .Where(f => !f.IsPaid) 
                .Sum(f => f.Amount);
            if (unpaidFinesAmount > 0)
                throw new Exception($"Cannot delete reader. Unpaid fines: {unpaidFinesAmount} ");
            
            reader.Status = ReaderState.DeletedByUser;

            readerRepository.Update(reader);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task BlockReader(int id, CancellationToken ct)
        {
            var reader = await readerRepository.GetByIdAsync(id, ct);
            if (reader == null) throw new Exception("Reader not found");

            if (reader.Status != ReaderState.Active)
                throw new Exception("Reader is not active");
            
            var hasActiveLoans = reader.BookBorrowings.Any(b => !b.IsReturned);
            if (hasActiveLoans)
                throw new Exception("Cannot delete reader while they have unreturned books.");
            
            var unpaidFinesAmount = reader.BookBorrowings
                .SelectMany(b => b.Fines) 
                .Where(f => !f.IsPaid) 
                .Sum(f => f.Amount);
            if (unpaidFinesAmount > 0)
                throw new Exception($"Cannot delete reader. Unpaid fines: {unpaidFinesAmount} ");
            
            reader.Status = ReaderState.Blocked;

            readerRepository.Update(reader);
            await unitOfWork.SaveChangesAsync(ct);
        }//To unblock needs to Pay Fines won't implement
    }
}
