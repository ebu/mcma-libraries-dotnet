using Amazon.Lambda;
using Amazon.Runtime;
using Amazon.Runtime.Credentials;

namespace Mcma.WorkerInvoker.Aws.Lambda;

public class LambdaWorkerInvokerOptions
{
    public string WorkerFunctionName { get; set; } = McmaLambdaWorkerInvokerEnvironmentVariables.WorkerFunctionName;
        
    public AWSCredentials Credentials { get; set; } = DefaultAWSCredentialsIdentityResolver.GetCredentials();
        
    public AmazonLambdaConfig Config { get; set; } = new();
}