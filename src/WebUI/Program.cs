using Application.Boopstrap;
using Application.Commond.Interface;
using Infrastructure.Boopstrap;
using NetEscapades.Configuration.Yaml;
using WebUI.Hubs;
using WebUI.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true)
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
        var origins = builder.Configuration.GetSection("CORS_Origins").Value;
        options.WithOrigins(origins.Split(","));
        options.AllowAnyHeader();
        options.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.EnvironmentName.Equals("Qa", StringComparison.OrdinalIgnoreCase))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();
