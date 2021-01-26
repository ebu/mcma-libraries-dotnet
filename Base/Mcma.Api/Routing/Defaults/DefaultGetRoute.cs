using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultGetRoute<TResource> : IDefaultRoute where TResource : McmaResource
    {
        public DefaultGetRoute(IDocumentDatabaseTableProvider dbTableProvider, string root)
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            HandleRequestAsync = DefaultHandleRequestAsync;
            
            Route = new McmaApiRoute(HttpMethod.Get, root + "/{id}", x => HandleRequestAsync(x));
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

            var table = await DbTableProvider.GetAsync(requestContext.EnvironmentVariables.TableName());

            // get the resource from the database
            var resource = await table.GetAsync<TResource>(requestContext.Request.Path);

            // invoke the completion handler, if any
            if (OnCompletedAsync != null)
                await OnCompletedAsync(requestContext, resource);

            // return the resource as json, if found; otherwise, this will return a 404
            if (resource != null)
                requestContext.SetResponseBody(resource);
            else
                requestContext.SetResponseResourceNotFound();
        }
    }
}