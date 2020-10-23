
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Mcma.Serialization;
using Mcma.WorkerInvoker;

namespace Mcma.Aws.WorkerInvoker
{
    public class LambdaWorkerInvoker : Mcma.WorkerInvoker.WorkerInvoker
    {
        public LambdaWorkerInvoker(IAmazonLambda lambdaClient = null, IEnvironmentVariables environmentVariables = null)
            : base(environmentVariables)
        {
            LambdaClient = lambdaClient ?? new AmazonLambdaClient();
        }
        
        private IAmazonLambda LambdaClient { get; }

        protected override async Task InvokeAsync(string workerFunctionId, WorkerRequest request)
        {
            // invoking worker lambda function that will handle the work for the service
            await LambdaClient.InvokeAsync(
                new InvokeRequest
                {
                    FunctionName = workerFunctionId,
                    InvocationType = "Event",
                    LogType = "None",
                    Payload = request.ToMcmaJson().ToString()
                }
            );
        }
    }
}