using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ApiGateway.Extensions;
using dotenv.net;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Fluent()
    .WithOverwriteExistingVars()
    .WithTrimValues()
    .WithProbeForEnv()
    .Load();
    
var base64Key = "JWT_KEY".FromEnvRequired();

var securityKeyBytes = Convert.FromBase64String(base64Key);
builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "JWT_ISSUER".FromEnvRequired(),
            ValidAudience ="JWT_AUDIENCE".FromEnvRequired(),
            IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytes)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GatewayAuthPolicy", policy => policy.RequireAuthenticatedUser());
});


var sharedSecret = "YARP_GATEWAY_KEY".FromEnvRequired();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformContext =>
    {
        // X-Org-Id is intentionally NOT handled here - it can't come from the JWT a user can
        // belong to multiple org, so it passes through from the client unchanged. Real org
        // access control happens in the Api project against real Membership rows.
        transformContext.AddRequestHeaderRemove("X-User-Id");
        transformContext.AddRequestHeaderRemove("X-Gateway-Secret");

        transformContext.AddRequestTransform(async context =>
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userId =
                    user.FindFirst("sub")?.Value ??
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
                    Console.WriteLine($"[Gateway] Added X-User-Id: {userId}");
                }
                else
                {
                    Console.WriteLine("AUTHENTICATED BUT NO USER ID CLAIM"); //just to check what's wrong ig i'll find
                    foreach (var c in user.Claims)
                        Console.WriteLine($"CLAIM: {c.Type} = {c.Value}");
                }
            }

            context.ProxyRequest.Headers.TryAddWithoutValidation("X-Gateway-Secret", sharedSecret);
            Console.WriteLine($"[Gateway] Added X-Gateway-Secret: {sharedSecret.Substring(0, 4)}...");//to find issue i'm logging in console
            await Task.CompletedTask;
        });
    });

var app = builder.Build();

app.UseRouting();
app.UseCors("GatewayCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();