namespace Application.DTOs
{
    public sealed record CreateMembershipPayloadDto
    {
        public string UserId { get; set; } = string.Empty;
        public string OrganisationId { get; set; } = string.Empty; 
        public string Role { get; set; } = string.Empty;
    }

    public sealed record DeleteMembershipPayloadDto
    {
        public string UserId { get; set; } = string.Empty;
        public string OrganisationId { get; set; } = string.Empty;
    }

    public sealed record MembershipBatchOperationDto
    {
        public List<CreateMembershipPayloadDto> ToCreate { get; set; } = [];
        public List<CreateMembershipPayloadDto> ToUpdate { get; set; } = [];
        public List<DeleteMembershipPayloadDto> ToDelete { get; set; } = [];
    }

    public sealed record BatchFailureInfo
    {
        public string Operation { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public sealed record MembershipBatchResultDto
    {
        public int Successes { get; set; }
        public List<BatchFailureInfo> Failures { get; set; } = [];
    }
}
