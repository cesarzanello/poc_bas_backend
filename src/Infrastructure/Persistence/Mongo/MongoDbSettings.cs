namespace Infrastructure.Persistence.Mongo
{
    public class MongoDbSettings
    {
        public const string SectionName = "MongoDb";

        public string ConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string TenantsCollectionName { get; set; } = "tenants";
    }
}
