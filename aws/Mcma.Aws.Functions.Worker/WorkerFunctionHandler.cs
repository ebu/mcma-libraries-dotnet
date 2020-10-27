using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Mcma.Logging;
using Mcma.Worker;

namespace Mcma.Aws.Functions.Worker
{
    public class WorkerFunctionHandler : IMcmaLambdaFunctionHandler<WorkerRequest>
    {
        public WorkerFunctionHandler(ILoggerProvider loggerProvider, IWorker worker)
        {
            LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
            Worker = worker ?? throw new ArgumentNullException(nameof(worker));
        }
     
        private ILoggerProvider LoggerProvider { get; }

        private IWorker Worker { get; }

        public async Task ExecuteAsync(WorkerRequest request, ILambdaContext context)
        {
            var logger = LoggerProvider.Get(context.AwsRequestId);

            try
            {
                logger.FunctionStart(context.AwsRequestId);
                logger.Debug(request);
                logger.Debug(context);

                await Worker.DoWorkAsync(new WorkerRequestContext(request, context.AwsRequestId));
            }
            finally
            {
                logger.FunctionEnd(context.AwsRequestId);
                await LoggerProvider.FlushAsync();
            }
        }
    }
}