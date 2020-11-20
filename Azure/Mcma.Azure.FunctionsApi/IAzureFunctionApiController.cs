﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace Mcma.Azure.FunctionsApi
{
    public interface IAzureFunctionApiController
    {
        Task<IActionResult> HandleRequestAsync(HttpRequest request, ExecutionContext executionContext);
    }
}