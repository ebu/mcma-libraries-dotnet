﻿using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbCustomQueryBuilderRegistry : ICustomQueryBuilderRegistry<QueryOperationConfig>
    {
        public DynamoDbCustomQueryBuilderRegistry(IEnumerable<ICustomQueryBuilder> queryBuilders)
        {
            QueryBuilders = queryBuilders?.ToArray() ?? new ICustomQueryBuilder[0];
        }

        private ICustomQueryBuilder[] QueryBuilders { get; }

        public ICustomQueryBuilder<TParameters, QueryOperationConfig> Get<TParameters>(string name)
            => QueryBuilders
                   .FirstOrDefault(
                       x =>
                           x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                           x.ParameterType == typeof(TParameters)) as ICustomQueryBuilder<TParameters, QueryOperationConfig>
               ?? throw new McmaException(
                   $"No custom query configured with name '{name}' that accepts parameters of type {typeof(TParameters).Name}");
    }
}