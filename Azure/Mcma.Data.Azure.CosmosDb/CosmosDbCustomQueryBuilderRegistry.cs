using System;
using System.Collections.Generic;
using System.Linq;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Microsoft.Azure.Cosmos;

namespace Mcma.Data.Azure.CosmosDb
{
    public class CosmosDbCustomQueryBuilderRegistry : ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)>
    {
        public CosmosDbCustomQueryBuilderRegistry(IEnumerable<ICustomQueryBuilder> queryBuilders)
        {
            QueryBuilders = queryBuilders?.ToArray() ?? new ICustomQueryBuilder[0];
        }

        private ICustomQueryBuilder[] QueryBuilders { get; }

        public ICustomQueryBuilder<TParameters, (QueryDefinition, QueryRequestOptions)> Get<TParameters>(string name)
            => QueryBuilders
                   .FirstOrDefault(
                       x =>
                           x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                           x.ParameterType == typeof(TParameters)) as ICustomQueryBuilder<TParameters, (QueryDefinition, QueryRequestOptions)>
               ?? throw new McmaException(
                   $"No custom query configured with name '{name}' that accepts parameters of type {typeof(TParameters).Name}");
    }
}