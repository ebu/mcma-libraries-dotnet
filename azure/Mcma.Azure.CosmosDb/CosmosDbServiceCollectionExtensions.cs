using System;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Azure.CosmosDb
{
    public static class CosmosDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaCosmosDb(this IServiceCollection services, Action<CosmosDbTableProviderOptions> configureOptions, Action<CosmosDbTableProviderBuilder> build = null)
        {
            services.Configure(configureOptions);
            
            var builder = new CosmosDbTableProviderBuilder(services);
            build?.Invoke(builder);
            
            services.TryAddSingleton<IQueryDefinitionBuilder, QueryDefinitionBuilder>();
            services.TryAddSingleton<ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)>, CustomQueryBuilderRegistry>();
            
            return services.AddSingleton<IDocumentDatabaseTableProvider, CosmosDbTableProvider>();
        }

        public static IServiceCollection AddMcmaCosmosDb(this IServiceCollection services, string endpoint, string key, string databaseId, string region)
            => services.AddMcmaCosmosDb(opts =>
            {
                opts.Endpoint = endpoint;
                opts.Key = key;
                opts.DatabaseId = databaseId;
                opts.CosmosClient.ApplicationRegion = region;
            });
    }
}