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
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
                }
            }

            context.ProxyRequest.Headers.TryAddWithoutValidation("X-Gateway-Secret", sharedSecret);

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