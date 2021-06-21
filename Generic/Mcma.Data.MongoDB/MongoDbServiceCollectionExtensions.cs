using System;
using Mcma.Data.DocumentDatabase;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB
{
    public static class MongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaMongoDb(this IServiceCollection services,
                                                        Action<MongoDbTableOptions> configureOptions = null,
                                                        Action<MongoDbBuilder> build = null)
        {
            ConventionRegistry.Register("MCMA Conventions", new ConventionPack {new CamelCaseElementNameConvention()}, _ => true);
            BsonSerializer.RegisterSerializer(new JObjectBsonSerializer());
            BsonClassMap.RegisterClassMap<McmaResourceDocument>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(x => x.Id);
            });
            
            if (configureOptions != null)
                services.Configure(configureOptions);

            var builder = new MongoDbBuilder(services);
            build?.Invoke(builder);

            services.TryAddSingleton<IMongoDbFilterDefinitionBuilder, MongoDbFilterDefinitionBuilder>();
            services.TryAddSingleton<ICustomQueryBuilderRegistry<FilterDefinition<McmaResourceDocument>>, MongoDbCustomQueryBuilderRegistry>();

            return services.AddSingleton<IDocumentDatabaseTable, MongoDbTable>();
        }

        public static IServiceCollection AddMcmaDynamoDb(this IServiceCollection services, string collectionName)
            => services.AddMcmaMongoDb(opts => opts.CollectionName = collectionName);
    }
}