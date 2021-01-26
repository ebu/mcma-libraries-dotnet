using System.Linq;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Api
{
    public static class QueryRequestHelper
    {
        public static Query<T> ToQuery<T>(this McmaApiRequestContext requestContext)
        {
            var filterExpression =
                requestContext.Request.QueryStringParameters.Any()
                    ? requestContext.Request.QueryStringParameters
                                    .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)
                                    .ToFilterExpression<T>()
                    : null;

            var pageStartToken = default(string);
            if (requestContext.Request.QueryStringParameters.ContainsKey(nameof(pageStartToken)))
                pageStartToken = requestContext.Request.QueryStringParameters[nameof(pageStartToken)];

            var pageSize = default(int?);
            if (requestContext.Request.QueryStringParameters.ContainsKey(nameof(pageSize)) &&
                int.TryParse(requestContext.Request.QueryStringParameters[nameof(pageSize)], out var pageSizeTemp))
                pageSize = pageSizeTemp;

            return new Query<T>
            {
                PageStartToken = pageStartToken,
                PageSize = pageSize,
                Path = requestContext.Request.Path,
                FilterExpression = filterExpression
            };
        }
    }
}