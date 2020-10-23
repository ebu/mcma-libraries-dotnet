using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultCreateRoute<TResource> : IDefaultRoute where TResource : McmaResource
    {
        public DefaultCreateRoute(IDocumentDatabaseTableProvider dbTableProvider, string root)
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            Root = root;
            HandleRequestAsync = DefaultHandleRequestAsync;
            
            Route = new McmaApiRoute(HttpMethod.Post, root, x => HandleRequestAsync(x));
        }
        
        internal McmaApiRoute Route { get; }

        McmaApiRoute IDefaultRoute.Route => Route;
        
        private string Root { get; }
        
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

            // ensure the body is set
            var resource = requestContext.GetRequestBody<TResource>();
            if (resource == null)
            {
                requestContext.SetResponseBadRequestDueToMissingBody();
                return;
            }

            var resourcePath = $"{Root}/{Guid.NewGuid()}";

            // initialize the new resource with an ID
            resource.OnCreate(requestContext.PublicUrlForPath(resourcePath));

            // put the new object into the table
            var table = await DbTableProvider.GetAsync(requestContext.EnvironmentVariables.TableName());
            await table.PutAsync(resourcePath, resource);

            // invoke the completion handler (if any) with the newly-created resource
            if (OnCompletedAsync != null)
                await OnCompletedAsync(requestContext, resource);

            // return a Created status with the id of the resource
            requestContext.SetResponseResourceCreated(resource);
        }
    }
}