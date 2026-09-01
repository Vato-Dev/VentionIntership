using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController(
        IUserService userService,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
        {
            //Since I don't have an attempt counter and the user blocking system depends on it I don't track the entity to increase performance
            var user = await userRepository.GetByEmailNoTrackingAsync(dto.Email, ct); 
 
           
            if (user == null || !passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var accessToken = tokenService.GenerateJwtToken(user);

            return Ok(new LoginResponseDto(
            user.Id.ToString(),
            user.Email,
            user.Name,
            user.Role,
            Image: null,
            AccessToken: accessToken));
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto, CancellationToken cancellationToken)
        {
            var createdUser = await userService.CreateUserAsync(dto, cancellationToken);
            return Ok(createdUser);
        }
    }

    public sealed record LoginRequestDto(string Email, string Password);
}
