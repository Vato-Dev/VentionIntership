using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using Api.Services;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))); //already connected database in previous tasks

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
var app = builder.Build();
app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{    app.UseCors(x=>x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    app.UseSwagger();
    app.UseSwaggerUI();
}

const string CacheKey = "todos:all";

app.MapGet("/todos", async (AppDbContext db, IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();
    var cached = await cache.StringGetAsync(CacheKey);

    if (cached.HasValue)
        return Results.Ok(JsonSerializer.Deserialize<List<TodoItem>>(cached!));

    var todos = await db.Todos.ToListAsync();
    await cache.StringSetAsync(CacheKey, JsonSerializer.Serialize(todos), TimeSpan.FromSeconds(30));
    return Results.Ok(todos);
});

app.MapGet("/todos/{id:int}", async (int id, AppDbContext db) =>
    await db.Todos.FindAsync(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound());

app.MapPost("/todos", async (TodoItem todo, AppDbContext db, IConnectionMultiplexer redis, RabbitMqPublisher publisher) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    await redis.GetDatabase().KeyDeleteAsync(CacheKey);
    publisher.PublishTodoCreated(todo.Id, todo.Title);

    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id:int}", async (int id, TodoItem input, AppDbContext db, IConnectionMultiplexer redis) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = input.Title;
    todo.IsDone = input.IsDone;
    await db.SaveChangesAsync();
    await redis.GetDatabase().KeyDeleteAsync(CacheKey);

    return Results.NoContent();
});

app.MapDelete("/todos/{id:int}", async (int id, AppDbContext db, IConnectionMultiplexer redis) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    await redis.GetDatabase().KeyDeleteAsync(CacheKey);

    return Results.NoContent();
});

app.Run();