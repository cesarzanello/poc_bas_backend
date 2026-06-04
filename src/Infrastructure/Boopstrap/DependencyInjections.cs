using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Boopstrap
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            var mongoSection = configuration.GetSection(MongoDbSettings.SectionName);
            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = mongoSection["ConnectionString"] ?? string.Empty;
                options.DatabaseName = mongoSection["DatabaseName"] ?? string.Empty;
                options.TenantsCollectionName = mongoSection["TenantsCollectionName"] ?? "tenants";
            });

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.DatabaseName);
            });

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<TenantDocument>(settings.TenantsCollectionName);
            });

            services.AddSingleton<MongoInitializer>();

            services.AddBackendScopedServices();

            return services;
        }
    }
}
