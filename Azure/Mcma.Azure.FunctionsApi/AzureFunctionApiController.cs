﻿using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace Mcma.Azure.FunctionsApi
{
    public class AzureFunctionApiController : IAzureFunctionApiController
    {
        public AzureFunctionApiController(IMcmaApiController controller, ILoggerProvider loggerProvider)
        {
            Controller = controller;
            LoggerProvider = loggerProvider;
        }

        private IMcmaApiController Controller { get; }

        private ILoggerProvider LoggerProvider { get; }

        public async Task<IActionResult> HandleRequestAsync(HttpRequest request, ExecutionContext executionContext)
        {
            var requestContext = await request.ToMcmaApiRequestContextAsync(LoggerProvider, executionContext);

            var logger = LoggerProvider?.Get(requestContext.RequestId, requestContext.GetTracker());

            logger?.Debug($"Starting {request.Method} request to {request.Path}...");

            await Controller.HandleRequestAsync(requestContext);

            logger?.Debug($"{request.Method} request to {request.Path} finished with status {requestContext.Response.StatusCode}");

            return requestContext.ToActionResult();
        }
    }
}
