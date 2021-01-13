using System.Threading.Tasks;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Api.Routing.Defaults.Routes
{
    internal interface IMcmaApiCustomQuery<T>
    {
        bool IsMatch(McmaApiRequestContext requestContext);

        Task<QueryResults<T>> ExecuteAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table);
    }
}