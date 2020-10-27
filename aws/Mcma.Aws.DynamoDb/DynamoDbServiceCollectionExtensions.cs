using System;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Aws.DynamoDb
{
    public static class DynamoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaDynamoDb(this IServiceCollection services, Action<DynamoDbTableProviderOptions> configureOptions = null, Action<DynamoDbTableProviderBuilder> build = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            build?.Invoke(new DynamoDbTableProviderBuilder(services));

            services.TryAddSingleton<IFilterExpressionBuilder, FilterExpressionBuilder>();
            services.TryAddSingleton<IAttributeMapper, AttributeMapper>();
            services.TryAddSingleton<ICustomQueryBuilderRegistry<QueryOperationConfig>, CustomQueryBuilderRegistry>();
            services.TryAddSingleton<ITableDescriptionProvider, TableDescriptionProvider>();
            
            return services.AddSingleton<IDocumentDatabaseTableProvider, DynamoDbTableProvider>();
        }
    }
}