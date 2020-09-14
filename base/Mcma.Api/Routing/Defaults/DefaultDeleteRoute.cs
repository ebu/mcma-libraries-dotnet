using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Context;
using Mcma.Data;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultDeleteRoute<TResource> : IDefaultRoute where TResource : McmaResource
    {
        public DefaultDeleteRoute(IDocumentDatabaseTableProvider dbTableProvider, string root)
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            HandleRequestAsync = DefaultHandleRequestAsync;
            
            Route = new McmaApiRoute(HttpMethod.Delete, root + "/{id}", x => HandleRequestAsync(x));
        }
        
        internal McmaApiRoute Route { get; }

        McmaApiRoute IDefaultRoute.Route => Route;
        
        private IDocumentDatabaseTableProvider DbTableProvider { get; }

        public Func<McmaApiRequestContext, Task<bool>> OnStartedAsync { get; set; }
        
        public Func<McmaApiRequestContext, TResource, Task> OnCompletedAsync { get; set; }
        
        public Func<McmaApiRequestContext, Task> HandleRequestAsync { get; set; }

        private async Task DefaultHandleRequestAsync(McmaApiRequestContext requestContext)
        {
            // invoke the start handler, if any
            if (OnStartedAsync != null)
                if (!await OnStartedAsync(requestContext))
                    return;

            // get the table for the resource
            var table = await DbTableProvider.GetAsync(requestContext.TableName());

            // build id from the root public url and the path
            var id = requestContext.Request.Path;

            // get the resource from the db
            var resource = await table.GetAsync<TResource>(id);

            // if the resource doesn't exist, return a 404
            if (resource == null)
            {
                requestContext.SetResponseResourceNotFound();
                return;
            }

            // delete the resource from the db
            await table.DeleteAsync(id);

            // invoke the completion handler, if any
            if (OnCompletedAsync != null)
                await OnCompletedAsync(requestContext, resource);
        }
    }
}