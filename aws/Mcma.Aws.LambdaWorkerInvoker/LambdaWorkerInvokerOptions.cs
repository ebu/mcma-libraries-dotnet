using Amazon.Lambda;
using Amazon.Runtime;
using Mcma.WorkerInvoker;

namespace Mcma.Aws.WorkerInvoker
{
    public class LambdaWorkerInvokerOptions : WorkerInvokerOptions
    {
        public AWSCredentials Credentials { get; set; }
        
        public AmazonLambdaConfig Config { get; set; }
    }
}