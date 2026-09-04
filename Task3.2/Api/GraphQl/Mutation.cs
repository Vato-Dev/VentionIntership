using Domain.Models;
using Persistence;

namespace Api.GraphQl
{
    public class Mutation
    {
        public async Task<FileModel> UpdateFileStatus(
            [Service] AppDbContext context,
            Guid id,
            FileStatus status,
            string? processingError = null)
        {
            var file = await context.Files.FindAsync(id);
            if (file == null)
                throw new GraphQLException("File not found");

            file.Status = status;
            file.ProcessingError = processingError;
            file.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return file;
        }
    }
}
