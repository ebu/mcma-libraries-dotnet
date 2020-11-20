﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mcma.GoogleCloud.HttpFunctionsApi
{
    public interface IHttpFunctionApiController
    {
        Task HandleRequestAsync(HttpContext httpContext);
    }
}