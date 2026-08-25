using Domain.Models;

namespace Application.Abstractions
{
    public interface ITokenService
    {
       public string GenerateJwtToken(User user);
    }
}
