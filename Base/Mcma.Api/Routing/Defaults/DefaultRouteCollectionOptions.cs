using Mcma.Model;
using Mcma.Utility;

namespace Mcma.Api.Routing.Defaults
{
    public class DefaultRouteCollectionOptions<TResource> where TResource : McmaResource
    {
        public DefaultRouteCollectionOptions()
        {
            Root = typeof(TResource).Name.CamelCaseToKebabCase().PluralizeKebabCase();
        }
        
        private string _root;

        public string Root
        {
            get => _root;
            set
            {
                value ??= string.Empty;
                if (!value.StartsWith("/"))
                    value = "/" + value;
                _root = value;
            }
        }
    }
}