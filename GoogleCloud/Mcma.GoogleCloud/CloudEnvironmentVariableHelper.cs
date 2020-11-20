﻿using System;

namespace Mcma.GoogleCloud
{
    public static class CloudEnvironmentVariableHelper
    {    
        public static bool IsAppEngine
            => (Environment.GetEnvironmentVariable("GAE_SERVICE") ?? Environment.GetEnvironmentVariable("GAE_MODULE_NAME")) != null;
        
        public static string AppEngineModuleId => Environment.GetEnvironmentVariable("GAE_SERVICE") ?? Environment.GetEnvironmentVariable("GAE_MODULE_NAME");

        public static string AppEngineVersionId => Environment.GetEnvironmentVariable("GAE_VERSION");

        public static bool IsCloudFunction
            => (Environment.GetEnvironmentVariable("FUNCTION_NAME") ?? Environment.GetEnvironmentVariable("FUNCTION_TARGET")) != null;

        public static string CloudFunctionName
            => Environment.GetEnvironmentVariable("K_SERVICE") ?? Environment.GetEnvironmentVariable("FUNCTION_TARGET");

        public static string CloudFunctionRegion
            => Environment.GetEnvironmentVariable("GOOGLE_CLOUD_REGION") ?? Environment.GetEnvironmentVariable("FUNCTION_REGION");
    }
}