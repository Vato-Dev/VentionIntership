using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames=Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Services
{
    public class TokenService(IOptions<JwtOptions> options) : ITokenService
    {
        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
 

            var key = new SymmetricSecurityKey(Convert.FromBase64String(options.Value.SecretKey));
            var signCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 
            var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.Value.ExpirationInMinutes),
            signingCredentials: signCreds);
 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
