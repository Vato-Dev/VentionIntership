namespace Api.Contracts
{
    public record RawDataIngested(Guid IngestionId, string RawPayload, DateTime IngestedAt);

}
