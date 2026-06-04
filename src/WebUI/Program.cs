using Application.Boopstrap;
using Application.Commond.Interface;
using Infrastructure.Boopstrap;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mongo;
using Microsoft.EntityFrameworkCore;
using NetEscapades.Configuration.Yaml;
using WebUI.Hubs;
using WebUI.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true)
    .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yml", optional: true, reloadOnChange: true);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
builder.Services.AddScoped<INotificationsFacade, NotificationsFacade>();
builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(options =>
    {
        var origins = builder.Configuration["CORS_Origins"] ?? "http://localhost:3000";
        options.WithOrigins(origins.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
        options.AllowAnyHeader();
        options.AllowAnyMethod();
        options.AllowCredentials();
    });
});

var app = builder.Build();
var isRunningInContainer = string.Equals(
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
    "true",
    StringComparison.OrdinalIgnoreCase);

const int startupRetries = 10;
var startupDelay = TimeSpan.FromSeconds(3);

using (var scope = app.Services.CreateScope())
{
    var mongoInitializer = scope.ServiceProvider.GetRequiredService<MongoInitializer>();
    await ExecuteWithRetryAsync(() => mongoInitializer.InitializeAsync(), startupRetries, startupDelay);

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await ExecuteWithRetryAsync(() => dbContext.Database.EnsureCreatedAsync(), startupRetries, startupDelay);
}

static async Task ExecuteWithRetryAsync(Func<Task> operation, int retries, TimeSpan delay)
{
    for (var attempt = 1; attempt <= retries; attempt++)
    {
        try
        {
            await operation();
            return;
        }
        catch when (attempt < retries)
        {
            await Task.Delay(delay);
        }
    }

    await operation();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.EnvironmentName.Equals("Qa", StringComparison.OrdinalIgnoreCase))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!isRunningInContainer)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();
