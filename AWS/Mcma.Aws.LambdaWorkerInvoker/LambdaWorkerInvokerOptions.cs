using Amazon.Lambda;
using Amazon.Runtime;
using Mcma.WorkerInvoker;

namespace Mcma.Aws.WorkerInvoker
{
    public class LambdaWorkerInvokerOptions
    {
        public string WorkerFunctionName { get; set; } = McmaLambdaWorkerInvokerEnvironmentVariables.WorkerFunctionName;
        
        public AWSCredentials Credentials { get; set; } = FallbackCredentialsFactory.GetCredentials();
        
        public AmazonLambdaConfig Config { get; set; } = new();
    }
}