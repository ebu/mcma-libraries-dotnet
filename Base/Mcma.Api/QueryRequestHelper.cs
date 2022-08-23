using System;
using System.Linq;
using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Api;

public static class QueryRequestHelper
{
    public static Query<T> ToQuery<T>(this McmaApiRequestContext requestContext)
    {
        var query = new Query<T>
        {
            Path = requestContext.Request.Path
        };

        if (!requestContext.Request.QueryStringParameters.Any())
            return query;

        var filters =
            requestContext.Request.QueryStringParameters.ToDictionary(kvp => kvp.Key,
                                                                      kvp => kvp.Value,
                                                                      StringComparer.OrdinalIgnoreCase);
            
        if (filters.ContainsKey(nameof(query.PageStartToken)))
        {
            query.PageStartToken = filters[nameof(query.PageStartToken)];
            filters.Remove(nameof(query.PageStartToken));
        }

        if (filters.ContainsKey(nameof(query.PageSize)) && int.TryParse(filters[nameof(query.PageSize)], out var pageSizeTemp))
        {
            query.PageSize = pageSizeTemp;
            filters.Remove(nameof(query.PageSize));
        }

        if (filters.Any())
            query.FilterExpression = filters.ToFilterExpression<T>();

        return query;
    }
}