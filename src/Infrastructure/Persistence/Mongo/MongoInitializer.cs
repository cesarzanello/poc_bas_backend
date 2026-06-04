using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Mongo
{
    public class MongoInitializer(
        IMongoClient mongoClient,
        IMongoDatabase mongoDatabase,
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<MongoInitializer> logger)
    {
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await WaitUntilAvailableAsync(cancellationToken);

            var collectionName = mongoDbSettings.Value.TenantsCollectionName;
            var existingCollections = await mongoDatabase
                .ListCollectionNames()
                .ToListAsync(cancellationToken);

            if (!existingCollections.Contains(collectionName, StringComparer.OrdinalIgnoreCase))
            {
                await mongoDatabase.CreateCollectionAsync(collectionName, cancellationToken: cancellationToken);
                logger.LogInformation("MongoDB collection '{CollectionName}' creada.", collectionName);
            }

            await EnsureDefaultTenantsAsync(collectionName, cancellationToken);
        }

        private async Task EnsureDefaultTenantsAsync(string collectionName, CancellationToken cancellationToken)
        {
            var tenantsCollection = mongoDatabase.GetCollection<TenantDocument>(collectionName);

            var existingTenantNames = await tenantsCollection
                .Find(_ => true)
                .Project(x => x.Nombre)
                .ToListAsync(cancellationToken);

            var tenantsToInsert = DefaultTenants
                .Where(x => !existingTenantNames.Contains(x.Nombre, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (tenantsToInsert.Count == 0)
            {
                return;
            }

            await tenantsCollection.InsertManyAsync(tenantsToInsert, cancellationToken: cancellationToken);
            logger.LogInformation("Se crearon {Count} tenants por defecto en MongoDB.", tenantsToInsert.Count);
        }

        private static readonly IReadOnlyCollection<TenantDocument> DefaultTenants =
        [
            new TenantDocument
            {
                Id = Guid.Parse("4c00f652-6495-4d72-a8cc-25f3b6a7f0f7"),
                Nombre = "BAS"
            },
            new TenantDocument
            {
                Id = Guid.Parse("2f1682ca-4f64-4ef9-9030-4ad4af5720d4"),
                Nombre = "BAS-2"
            }
        ];

        private async Task WaitUntilAvailableAsync(CancellationToken cancellationToken)
        {
            const int maxAttempts = 10;
            var delay = TimeSpan.FromSeconds(2);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await mongoClient
                        .GetDatabase("admin")
                        .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning(ex, "MongoDB no disponible. Reintento {Attempt}/{MaxAttempts}...", attempt, maxAttempts);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new InvalidOperationException("No fue posible conectar con MongoDB durante el arranque.");
        }
    }
}
