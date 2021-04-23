using System;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableBuilder
    {
        public CosmosDbTableBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        private IServiceCollection Services { get; }

        public CosmosDbTableBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
            where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, (QueryDefinition, QueryRequestOptions)>
        {
            Services.AddCustomQueryBuilder<TParameters, (QueryDefinition, QueryRequestOptions), TCustomQueryBuilder>();
            return this;
        }
    }
}