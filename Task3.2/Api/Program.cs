using Application.SericeCollectionExtension;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.ServiceCollectionExtension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddPersistence();
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DatabaseSeeder.SeedData(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}"); //log in console since i had error and it helped to define it
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
//todo: delete real creds from appsettings