using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;

namespace Mcma.Api.Routing
{
    public interface IMcmaApiRoute
    {
        HttpMethod HttpMethod { get; }

        bool IsMatch(string path, out IDictionary<string, object> pathVariables);

        Task HandleAsync(McmaApiRequestContext requestContext);
    }
}