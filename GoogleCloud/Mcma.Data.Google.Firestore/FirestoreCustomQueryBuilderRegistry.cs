using System;
using System.Collections.Generic;
using System.Linq;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using FirestoreQuery = Google.Cloud.Firestore.Query;

namespace Mcma.Data.Google.Firestore;

public class FirestoreCustomQueryBuilderRegistry : ICustomQueryBuilderRegistry<(FirestoreQuery, string)>
{
    public FirestoreCustomQueryBuilderRegistry(IEnumerable<ICustomQueryBuilder> queryBuilders)
    {
        QueryBuilders = queryBuilders?.ToArray() ?? [];
    }

    private ICustomQueryBuilder[] QueryBuilders { get; }

    public ICustomQueryBuilder<TParameters, (FirestoreQuery, string)> Get<TParameters>(string name)
        => QueryBuilders
               .FirstOrDefault(
                   x =>
                       x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                       x.ParameterType == typeof(TParameters)) as ICustomQueryBuilder<TParameters, (FirestoreQuery, string)>
           ?? throw new McmaException(
               $"No custom query configured with name '{name}' that accepts parameters of type {typeof(TParameters).Name}");
}