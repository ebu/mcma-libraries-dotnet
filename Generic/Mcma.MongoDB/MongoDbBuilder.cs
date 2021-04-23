using System;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Mcma.MongoDb
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