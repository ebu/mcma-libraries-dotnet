using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Mcma.Logging;
using Mcma.Worker.Common;

namespace Mcma.Functions.Aws.Worker;

public class McmaLambdaWorker : IMcmaLambdaFunctionHandler<McmaWorkerRequest>
{
    public McmaLambdaWorker(ILoggerProvider loggerProvider, IMcmaWorker mcmaWorker)
    {
        LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        McmaWorker = mcmaWorker ?? throw new ArgumentNullException(nameof(mcmaWorker));
    }
     
    private ILoggerProvider LoggerProvider { get; }

    private IMcmaWorker McmaWorker { get; }

    public async Task ExecuteAsync(McmaWorkerRequest request, ILambdaContext context)
    {
        var logger = LoggerProvider.Get(context.AwsRequestId);

        try
        {
            logger.FunctionStart(context.AwsRequestId);
            logger.Debug(request);
            logger.Debug(context);

            await McmaWorker.DoWorkAsync(request, context.AwsRequestId);
        }
        finally
        {
            logger.FunctionEnd(context.AwsRequestId);
            await LoggerProvider.FlushAsync();
        }
    }
}