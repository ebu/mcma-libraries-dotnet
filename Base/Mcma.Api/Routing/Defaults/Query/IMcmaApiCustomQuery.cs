using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query
{
    internal interface IMcmaApiCustomQuery<T>
    {
        bool IsMatch(McmaApiRequestContext requestContext);

        Task<QueryResults<T>> ExecuteAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table);
    }
}