using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Context;
using Mcma.Data;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultUpdateRoute<TResource> : IDefaultRoute where TResource : McmaResource
    {
        public DefaultUpdateRoute(IDocumentDatabaseTableProvider dbTableProvider, string root)
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            HandleRequestAsync = DefaultHandleRequestAsync;
            
            Route = new McmaApiRoute(HttpMethod.Put, root + "/{id}", x => HandleRequestAsync(x));
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

            // ensure the body is set
            var resource = requestContext.GetRequestBody<TResource>();
            if (resource == null)
            {
                requestContext.SetResponseBadRequestDueToMissingBody();
                return;
            }

            // set properties for upsert
            resource.OnUpsert(requestContext.CurrentRequestPublicUrl());

            // upsert the resource
            var table = await DbTableProvider.GetAsync(requestContext.TableName());
            await table.PutAsync(requestContext.Request.Path, resource);

            // invoke the completion handler, if any
            if (OnCompletedAsync != null)
                await OnCompletedAsync(requestContext, resource);

            // return the new or updated resource as json
            requestContext.SetResponseBody(resource);
        }
    }
}