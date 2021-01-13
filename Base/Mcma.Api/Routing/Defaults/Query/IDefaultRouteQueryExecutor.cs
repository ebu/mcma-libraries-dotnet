using System.Threading.Tasks;
using Mcma.Data;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultRouteQueryExecutor<TResource>
    {
        Task<QueryResults<TResource>> ExecuteQueryAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table);
    }
}