namespace Api.Contracts
{
    public record DataValidated(Guid IngestionId, string CleanedPayload, bool IsValid);
}
