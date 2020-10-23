using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultQueryRoute<TResource> : IDefaultRoute where TResource : McmaResource
    {
        public DefaultQueryRoute(IDocumentDatabaseTableProvider dbTableProvider, string root)
        {
            DbTableProvider = dbTableProvider;
            ExecuteQueryAsync = DefaultExecuteQueryAsync;
            HandleRequestAsync = DefaultHandleRequestAsync;
            
            Route = new McmaApiRoute(HttpMethod.Get, root, x => HandleRequestAsync(x));
        }
        
        internal McmaApiRoute Route { get; }

        McmaApiRoute IDefaultRoute.Route => Route;
        
        private List<IMcmaApiCustomQuery<TResource>> CustomQueries { get; } = new List<IMcmaApiCustomQuery<TResource>>();
        
        private IDocumentDatabaseTableProvider DbTableProvider { get; }

        public Func<McmaApiRequestContext, Task<bool>> OnStartedAsync { get; set; }
        
        public Func<McmaApiRequestContext, IDocumentDatabaseTable, Task<QueryResults<TResource>>> ExecuteQueryAsync { get; set; } 
        
        public Func<McmaApiRequestContext, QueryResults<TResource>, Task> OnCompletedAsync { get; set; }
        
        public Func<McmaApiRequestContext, Task> HandleRequestAsync { get; set; }

        public DefaultQueryRoute<TResource> AddCustomQuery<TParameters>(
            Func<McmaApiRequestContext, bool> isMatch,
            Func<McmaApiRequestContext, CustomQuery<TParameters>> createQuery)
        {
            CustomQueries.Add(new McmaApiCustomQuery<TResource, TParameters>(isMatch, createQuery));
            return this;
        }

        private async Task DefaultHandleRequestAsync(McmaApiRequestContext requestContext)
        {
            // invoke the start handler, if any
            if (OnStartedAsync != null)
                if (!await OnStartedAsync(requestContext))
                    return;

            var table = await DbTableProvider.GetAsync(requestContext.EnvironmentVariables.TableName());

            // get all resources from the table, applying in-memory filtering using the query string (if any)
            var results = await ExecuteQueryAsync(requestContext, table);

            // invoke the completion handler with the results
            if (OnCompletedAsync != null)
                await OnCompletedAsync(requestContext, results);

            // return the results as JSON in the body of the response
            // NOTE: This will never return a 404 - just an empty collection
            requestContext.SetResponseBody(results);
        }

        private async Task<QueryResults<TResource>> DefaultExecuteQueryAsync(McmaApiRequestContext requestContext, IDocumentDatabaseTable table)
        {
            var customQuery = CustomQueries.FirstOrDefault(x => x.IsMatch(requestContext));
            if (customQuery != null)
                return await customQuery.ExecuteAsync(requestContext, table);
            
            return await table.QueryAsync(requestContext.ToQuery<TResource>());
        }
    }
}