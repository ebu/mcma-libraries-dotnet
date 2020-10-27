using System;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTableProviderBuilder
    {
        public DynamoDbTableProviderBuilder(IServiceCollection services)
        {
            Services = services;
        }
        
        private IServiceCollection Services { get; }

        public DynamoDbTableProviderBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
            where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, QueryOperationConfig>
        {
            Services.AddSingleton<ICustomQueryBuilder<TParameters, QueryOperationConfig>, TCustomQueryBuilder>();
            return this;
        }

        public DynamoDbTableProviderBuilder AddAttributeMapping<TAttributeMapping>() where TAttributeMapping : class, IAttributeMapping
        {
            Services.AddSingleton<IAttributeMapping, TAttributeMapping>();
            return this;
        }

        public DynamoDbTableProviderBuilder AddAttributeMapping<TResource>(string name, Func<string, string, TResource, object> get)
        {
            Services.AddSingleton<IAttributeMapping>(new AttributeMapping<TResource>(name, get));
            return this;
        }
    }
}