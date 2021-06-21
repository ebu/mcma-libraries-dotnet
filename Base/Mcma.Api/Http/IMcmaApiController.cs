using System.Threading.Tasks;

namespace Mcma.Api.Http
{
    public interface IMcmaApiController
    {
        Task HandleRequestAsync(McmaApiRequestContext requestContext);
    }
}