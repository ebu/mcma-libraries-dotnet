using System;
using Mcma.Data;
using Mcma.Data.DocumentDatabase;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Data.Azure.CosmosDb
{
    public static class CosmosDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaCosmosDb(this IServiceCollection services,
                                                         Action<CosmosDbTableOptions> configureOptions = null,
                                                         Action<CosmosDbTableBuilder> build = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            var builder = new CosmosDbTableBuilder(services);
            build?.Invoke(builder);

            services.TryAddSingleton<IQueryDefinitionBuilder, QueryDefinitionBuilder>();
            services.TryAddSingleton<ICosmosDbContainerProvider, CosmosDbContainerProvider>();
            services.TryAddSingleton<ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)>, CosmosDbCustomQueryBuilderRegistry>();

            return services.AddSingleton<IDocumentDatabaseTable, CosmosDbTable>();
        }
    }
}