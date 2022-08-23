using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query;

public interface IDefaultRouteQueryExecutor<TResource>
{
    Task<QueryResults<TResource>> ExecuteQueryAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table);
}