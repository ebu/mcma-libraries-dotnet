﻿using System;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Aws.DynamoDb
{
    public static class DynamoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaDynamoDb(this IServiceCollection services,
                                                         Action<DynamoDbTableOptions> configureOptions = null,
                                                         Action<DynamoDbTableBuilder> build = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            build?.Invoke(new DynamoDbTableBuilder(services));

            services.TryAddSingleton<IFilterExpressionBuilder, FilterExpressionBuilder>();
            services.TryAddSingleton<IAttributeMapper, AttributeMapper>();
            services.TryAddSingleton<ICustomQueryBuilderRegistry<QueryOperationConfig>, DynamoDbCustomQueryBuilderRegistry>();
            services.TryAddSingleton<ITableDescriptionProvider, TableDescriptionProvider>();

            return services.AddSingleton<IDocumentDatabaseTable, DynamoDbTable>();
        }

        public static IServiceCollection AddMcmaDynamoDb(this IServiceCollection services,
                                                         string tableName,
                                                         Action<DynamoDbTableBuilder> build = null)
            => services.AddMcmaDynamoDb(opts => opts.TableName = tableName, build);
    }
}