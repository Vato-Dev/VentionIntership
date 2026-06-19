namespace LibraryManager.Application.DTO_s.Requests
{
    public sealed record AddReaderRequest(string Name, string PersonalNumber, string PhoneNumber, string EmailAddress);

}
