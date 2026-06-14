using Api.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<FacultyOptions>(builder.Configuration.GetSection(FacultyOptions.Faculty)); //it's an default one //has Singleton Lifetime

#region Named Configuration

builder.Services.AddOptions<FacultyOptions>("Faculty first")
    .Configure(options => options.Title = "engineering");

builder.Services.AddOptions<FacultyOptions>("Faculty second")
    .Configure(options => options.Title = "diplomacy");
#endregion

# region PostConfigure
//i left it empty in Appsettings on purpose
builder.Services.PostConfigureAll<FacultyOptions>(options => // I never knew that PostConfigure does not work on named options
{
    if (string.IsNullOrEmpty(options.Administrator))
    {
        options.Administrator = "Default(Post configuresd)";
    }
});
#endregion
var app = builder.Build();

app.MapGet("/options", (IOptions<FacultyOptions> options) =>
{
    return Results.Ok(new { 
        Source = "IOptions (Singleton)", 
        options.Value.Title, 
        options.Value.Administrator 
    });
});

app.MapGet("/options-snapshot/{name}", (string name, IOptionsSnapshot<FacultyOptions> snapshot) =>
{
    var faculty = snapshot.Get(name);
    
    return Results.Ok(new { 
        Source = $"IOptionsSnapshot : {name}", //it's scoped 
        faculty.Title, 
        faculty.Administrator // if i do not write anything there it can be filled from post configuring
    });
});


app.MapGet("/options-monitor/{name}", (string name, IOptionsMonitor<FacultyOptions> monitor) =>
{
    var faculty = monitor.Get(name);
    
    return Results.Ok(new { 
        Source = $"IOptionsMonitor : {name}", 
        faculty.Title, 
        faculty.Administrator 
    });
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

