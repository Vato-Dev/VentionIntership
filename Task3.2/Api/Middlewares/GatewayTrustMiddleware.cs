using System.Security.Cryptography;
using System.Text;
using Domain.Extensions;

namespace Api.Middlewares
{
    public sealed class GatewayTrustMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly byte[] _expectedSecretBytes = Encoding.UTF8.GetBytes("YARP_GATEWAY_KEY".FromEnvRequired());

        public async Task InvokeAsync(HttpContext context)
        {
            var inboundSecret = context.Request.Headers["X-Gateway-Secret"].FirstOrDefault();

            if (string.IsNullOrEmpty(inboundSecret))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Direct access to API is prohibited. Use reverse proxy API Gateway." });
                return;
            }

            var inboundSecretBytes = Encoding.UTF8.GetBytes(inboundSecret);

            if (!CryptographicOperations.FixedTimeEquals(_expectedSecretBytes, inboundSecretBytes))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Invalid internal gateway token signature." });
                return;
            }

            context.Request.Headers.Remove("X-Gateway-Secret");

            await next(context);
        }
    }
}
