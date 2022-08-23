using System.Threading.Tasks;
using Mcma.Api.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mcma.Api.Azure.FunctionApp;

public class McmaApiActionResult : ActionResult
{
    public McmaApiActionResult(McmaApiResponse response)
    {
        Response = response;
    }

    private McmaApiResponse Response { get; }

    public override Task ExecuteResultAsync(ActionContext context) => Response.CopyToHttpResponseAsync(context.HttpContext.Response);
}