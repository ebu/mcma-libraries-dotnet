﻿using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableBuilder
    {
        public CosmosDbTableBuilder(IServiceCollection services)
        {
            Services = services;
        }
        
        private IServiceCollection Services { get; }

        public CosmosDbTableBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
            where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, (QueryDefinition, QueryRequestOptions)>
        {
            Services.AddSingleton<ICustomQueryBuilder<TParameters, (QueryDefinition, QueryRequestOptions)>, TCustomQueryBuilder>();
            return this;
        }
    }
}