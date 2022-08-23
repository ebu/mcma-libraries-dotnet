using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Mcma.Serialization;
using Mcma.Worker.Common;
using Microsoft.Extensions.Options;

namespace Mcma.WorkerInvoker.Aws.Lambda;

public class LambdaMcmaWorkerInvoker : McmaWorkerInvoker
{
    public LambdaMcmaWorkerInvoker(IOptions<LambdaWorkerInvokerOptions> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value?.WorkerFunctionName))
            throw new McmaException("Worker function not configured");

        WorkerFunctionName = options.Value?.WorkerFunctionName;
        LambdaClient = new AmazonLambdaClient(options.Value?.Credentials, options.Value?.Config);
    }

    private string WorkerFunctionName { get; }

    private IAmazonLambda LambdaClient { get; }

    protected override async Task InvokeAsync(McmaWorkerRequest request)
    {
        // invoking worker lambda function that will handle the work for the service
        await LambdaClient.InvokeAsync(
            new InvokeRequest
            {
                FunctionName = WorkerFunctionName,
                InvocationType = "Event",
                LogType = "None",
                Payload = request.ToMcmaJson().ToString()
            }
        );
    }
}