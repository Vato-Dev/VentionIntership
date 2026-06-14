namespace LibraryManager.Application.DTO_s.Requests
{
    public sealed class UpdateReaderRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PersonalNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
    }
}
