namespace Infrastructure.Services
{
    public class JwtOptions
    {
        public const string Jwt = "Jwt";
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; }= null!;
        public string Audience { get; set; }= null!;
        public int ExpirationInMinutes { get; set; } = 15;
    }
}
