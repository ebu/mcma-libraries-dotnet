using Amazon.Lambda;
using Amazon.Runtime;
using Mcma.WorkerInvoker;

namespace Mcma.Aws.WorkerInvoker
{
    public class LambdaWorkerInvokerOptions : WorkerInvokerOptions
    {
        public AWSCredentials Credentials { get; set; } = FallbackCredentialsFactory.GetCredentials();
        
        public AmazonLambdaConfig Config { get; set; } = new AmazonLambdaConfig();
    }
}