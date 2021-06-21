using System;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB
{
    public class MongoDbBuilder
    {
        public MongoDbBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
        public IServiceCollection Services { get; }

        public MongoDbBuilder AddCustomQueryBuilder<TParameters, TCustomQueryBuilder>()
            where TCustomQueryBuilder : class, ICustomQueryBuilder<TParameters, FilterDefinition<McmaResourceDocument>>
        {
            Services.AddCustomQueryBuilder<TParameters, FilterDefinition<McmaResourceDocument>, TCustomQueryBuilder>();
            return this;
        }
    }
}