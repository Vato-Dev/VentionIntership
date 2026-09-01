using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.Extensions.Options;

namespace Api.WebAppBuilderExtensions
{
    public sealed class GatewayTrustOptions : AuthenticationSchemeOptions
    {
        public string SharedSecret { get; set; } = default!;
    }

    public sealed class GatewayTrustHandler(
        IOptionsMonitor<GatewayTrustOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<GatewayTrustOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Gateway-Secret", out var providedSecretValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing gateway secret"));
            }

            var providedBytes = Encoding.UTF8.GetBytes(providedSecretValues.ToString());
            var expectedBytes = Encoding.UTF8.GetBytes(Options.SharedSecret);

            if (!CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid gateway secret"));
            }


            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                var anonymousIdentity = new ClaimsIdentity(Scheme.Name);
                var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
                var anonymousTicket = new AuthenticationTicket(anonymousPrincipal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(anonymousTicket));
            }

            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdValues)
                || !Guid.TryParse(userIdValues.ToString(), out var userId))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing or invalid X-User-Id"));
            }

            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };

            if (Request.Headers.TryGetValue("X-Org-Id", out var orgIdValues)
                && Guid.TryParse(orgIdValues.ToString(), out var orgId))
            {
                claims.Add(new Claim("org_id", orgId.ToString()));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
