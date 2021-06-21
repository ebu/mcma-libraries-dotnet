using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mcma.Api.Google.CloudFunctions
{
    public interface IHttpFunctionApiController
    {
        Task HandleRequestAsync(HttpContext httpContext);
    }
}