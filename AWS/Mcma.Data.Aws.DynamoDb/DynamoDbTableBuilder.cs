using System;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.Aws.DynamoDb.AttributeMapping;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Data.Aws.DynamoDb;

public class DynamoDbTableBuilder
{
    public DynamoDbTableBuilder(IServiceCollection services)
    {
        Services = services;
    }
        
    private IServiceCollection Services { get; }

    public DynamoDbTableBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
        where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, QueryOperationConfig>
    {
        Services.AddCustomQueryBuilder<TParameters, QueryOperationConfig, TCustomQueryBuilder>();
        return this;
    }

    public DynamoDbTableBuilder AddAttributeMapping<TAttributeMapping>() where TAttributeMapping : class, IAttributeMapping
    {
        Services.AddSingleton<IAttributeMapping, TAttributeMapping>();
        return this;
    }

    public DynamoDbTableBuilder AddAttributeMapping<TResource>(string name, Func<string, string, TResource, object> get)
    {
        Services.AddSingleton<IAttributeMapping>(new AttributeMapping<TResource>(name, get));
        return this;
    }
}