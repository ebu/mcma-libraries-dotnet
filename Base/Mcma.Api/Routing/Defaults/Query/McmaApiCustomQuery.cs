using System;
using System.Threading.Tasks;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class McmaApiCustomQuery<TResource, TParameters> : IMcmaApiCustomQuery<TResource>
        where TResource : class
    {
        public McmaApiCustomQuery(Func<McmaApiRequestContext, bool> isMatch, Func<McmaApiRequestContext, CustomQuery<TParameters>> createQuery)
        {
            IsMatch = isMatch ?? throw new ArgumentNullException(nameof(isMatch));
            CreateQuery = createQuery ?? throw new ArgumentNullException(nameof(createQuery));
        }
        
        private Func<McmaApiRequestContext, bool> IsMatch { get; }

        private Func<McmaApiRequestContext, CustomQuery<TParameters>> CreateQuery { get; }

        bool IMcmaApiCustomQuery<TResource>.IsMatch(McmaApiRequestContext requestContext) => IsMatch(requestContext);

        public Task<QueryResults<TResource>> ExecuteAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table)
        {
            var customQuery = CreateQuery(requestContext);

            return table.CustomQueryAsync<TResource, TParameters>(customQuery);
        }
    }
}