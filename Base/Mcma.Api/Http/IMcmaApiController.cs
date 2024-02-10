namespace Mcma.Api.Http;

public interface IMcmaApiController
{
    Task HandleRequestAsync(McmaApiRequestContext requestContext);
}