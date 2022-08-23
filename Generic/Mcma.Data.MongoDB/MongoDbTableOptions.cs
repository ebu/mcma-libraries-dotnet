using Mcma.Data.DocumentDatabase;
using Mcma.Utility;

namespace Mcma.Data.MongoDB;

public class MongoDbTableOptions : DocumentDatabaseTableOptions
{
    public string ConnectionString { get; set; } = McmaEnvironmentVariables.Get("MONGODB_CONNECTION_STRING", false);

    public string DatabaseName { get; set; } = McmaEnvironmentVariables.Get("MONGODB_DATABASE_NAME", false);

    public string CollectionName { get; set; } = McmaEnvironmentVariables.Get("MONGODB_COLLECTION_NAME", false);
}