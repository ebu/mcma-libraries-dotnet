using System.Linq;
using Mcma.Api.Routes;
using Mcma.Api.Routing.Defaults.Routes;
using Mcma.Data;
using Mcma.Utility;

namespace Mcma.Api.Routing.Defaults
{
    public class DefaultRouteCollection<TResource> : McmaApiRouteCollection where TResource : McmaResource
    {
        public DefaultRouteCollection(IDocumentDatabaseTableProvider dbTableProvider, string root = null)
        {
            root = root ?? typeof(TResource).Name.CamelCaseToKebabCase().PluralizeKebabCase();
            if (!root.StartsWith("/"))
                root = "/" + root;
            
            Query = new DefaultQueryRoute<TResource>(dbTableProvider, root);
            Create = new DefaultCreateRoute<TResource>(dbTableProvider, root);
            Get = new DefaultGetRoute<TResource>(dbTableProvider, root);
            Update = new DefaultUpdateRoute<TResource>(dbTableProvider, root);
            Delete = new DefaultDeleteRoute<TResource>(dbTableProvider, root);

            AddRoute(Query.Route);
            AddRoute(Create.Route);
            AddRoute(Get.Route);
            AddRoute(Update.Route);
            AddRoute(Delete.Route);
        }

        public DefaultQueryRoute<TResource> Query { get; }

        public DefaultCreateRoute<TResource> Create { get; }

        public DefaultGetRoute<TResource> Get { get; }

        public DefaultUpdateRoute<TResource> Update { get; }

        public DefaultDeleteRoute<TResource> Delete { get; }

        public DefaultRouteCollection<TResource> Remove(IDefaultRoute defaultRoute)
        {
            var routeToRemove = Routes.FirstOrDefault(r => r == defaultRoute.Route);
            if (routeToRemove != null)
                Routes.Remove(routeToRemove);
            return this;
        }
    }
}
