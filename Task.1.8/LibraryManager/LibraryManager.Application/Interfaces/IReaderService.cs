using LibraryManager.Application.DTO_s.Requests;

namespace LibraryManager.Application.Interfaces
{
    public interface IReaderService
    {
        Task<int> CreateReader(AddReaderRequest request, CancellationToken ct);
        Task UpdateReaderProfile(UpdateReaderRequest request, CancellationToken ct);
        Task DeleteReader(int id, CancellationToken ct);
        Task BlockReader(int id, CancellationToken ct);
    }
}
