﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB;

public class MongoDbCustomQueryBuilderRegistry : ICustomQueryBuilderRegistry<FilterDefinition<McmaResourceDocument>>
{
    public MongoDbCustomQueryBuilderRegistry(IEnumerable<ICustomQueryBuilder> queryBuilders)
    {
        QueryBuilders = queryBuilders?.ToArray() ?? new ICustomQueryBuilder[0];
    }

    private ICustomQueryBuilder[] QueryBuilders { get; }

    public ICustomQueryBuilder<TParameters, FilterDefinition<McmaResourceDocument>> Get<TParameters>(string name)
        => QueryBuilders
               .FirstOrDefault(
                   x =>
                       x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                       x.ParameterType == typeof(TParameters)) as ICustomQueryBuilder<TParameters, FilterDefinition<McmaResourceDocument>>
           ?? throw new McmaException(
               $"No custom query configured with name '{name}' that accepts parameters of type {typeof(TParameters).Name}");
}