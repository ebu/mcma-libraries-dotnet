﻿using Mcma.Utility;

namespace Mcma.Aws.WorkerInvoker
{
    public static class McmaLambdaWorkerInvokerEnvironmentVariables
    {
        public static string WorkerFunctionName => McmaEnvironmentVariables.Get("WORKER_LAMBDA_FUNCTION_NAME", false);
    }
}