﻿using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Mcma.Logging;
using Mcma.Worker;

namespace Mcma.Aws.Functions.Worker
{
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

                await McmaWorker.DoWorkAsync(new McmaWorkerRequestContext(request, context.AwsRequestId));
            }
            finally
            {
                logger.FunctionEnd(context.AwsRequestId);
                await LoggerProvider.FlushAsync();
            }
        }
    }
}