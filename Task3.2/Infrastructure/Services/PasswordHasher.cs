using System;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions;
using Domain.Extensions;

namespace Infrastructure.Services
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private readonly byte[] _pepperBytes;

        public PasswordHasher()
        {
            var pepper = "PEPPER".FromEnvRequired(); //it will cache it in memory
            _pepperBytes = Encoding.UTF8.GetBytes(pepper);
        }

        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(GetHashedPepperedPassword(password), 11);

        public bool VerifyPassword(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(GetHashedPepperedPassword(password), hashedPassword);

        private string GetHashedPepperedPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            using var hmac = new HMACSHA256(_pepperBytes);
            var hash = hmac.ComputeHash(passwordBytes);
            
            return Convert.ToBase64String(hash);
        }
    }
}
