using System.Threading.Tasks;
using Mcma.Logging;

namespace Mcma.Api
{
    public interface IMcmaApiController
    {
        Task HandleRequestAsync(McmaApiRequestContext requestContext);
    }
}