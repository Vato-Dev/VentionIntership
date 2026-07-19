using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var jwtSecretKey = Encoding.UTF8.GetBytes("cG9ja2V0YW50c2NvbXBhcmVhbGlrZWZpcmVjcmVhdGVhZHZpY2VzaXR1YXRpb25mZWQ=!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "ECommerceAuthServer",
            ValidAudience = "ECommerceClients",
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GatewayJwtPolicy", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformContext =>
    {
        transformContext.AddRequestTransform(async tik =>
        {
            var user = tik.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.Identity.Name ?? "UnknownUser";
                
                tik.ProxyRequest.Headers.Remove("X-User-Id");
                tik.ProxyRequest.Headers.Add("X-User-Id", userId);
            }
            await Task.CompletedTask;
        });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/auth/token", () =>
{
    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "student-user-777") }),
        Expires = DateTime.UtcNow.AddHours(2),
        Issuer = "ECommerceAuthServer",
        Audience = "ECommerceClients",
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecretKey), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = handler.CreateToken(tokenDescriptor);
    return Results.Ok(new { access_token = handler.WriteToken(token) });
});

app.MapReverseProxy();

app.Run();
