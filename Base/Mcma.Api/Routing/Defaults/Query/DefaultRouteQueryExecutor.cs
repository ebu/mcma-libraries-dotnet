using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query;

internal class DefaultRouteQueryExecutor<TResource> : IDefaultRouteQueryExecutor<TResource> where TResource : McmaResource
{
    public DefaultRouteQueryExecutor(IEnumerable<IMcmaApiCustomQuery<TResource>> customQueries)
    {
        CustomQueries = customQueries?.ToArray() ?? [];
    }

    private IMcmaApiCustomQuery<TResource>[] CustomQueries { get; }

    public async Task<QueryResults<TResource>> ExecuteQueryAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table)
    {
        var customQuery = CustomQueries.FirstOrDefault(x => x.IsMatch(requestContext));
        if (customQuery != null)
            return await customQuery.ExecuteAsync(requestContext, table);
            
        return await table.QueryAsync(requestContext.ToQuery<TResource>());
    }
}