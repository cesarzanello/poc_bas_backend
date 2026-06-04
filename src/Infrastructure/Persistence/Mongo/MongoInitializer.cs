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
        }

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
