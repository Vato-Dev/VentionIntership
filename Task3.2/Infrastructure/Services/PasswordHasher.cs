using Application.Abstractions;

namespace Infrastructure.Services
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        =>  BCrypt.Net.BCrypt.HashPassword(password, 11);// time increases so much so i will stop at 11 iterations

        public bool VerifyPassword(string password, string hashedPassword)
         =>  BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
