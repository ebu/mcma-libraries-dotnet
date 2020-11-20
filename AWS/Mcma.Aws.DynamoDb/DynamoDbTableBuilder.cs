﻿using System;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.DynamoDb
{
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
            Services.AddSingleton<ICustomQueryBuilder<TParameters, QueryOperationConfig>, TCustomQueryBuilder>();
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
}