﻿using Mcma.Utility;

namespace Mcma.Client
{
    public static class McmaResourceManagerEnvironmentVariables
    {
        public static string ServicesUrl => McmaEnvironmentVariables.Get("SERVICES_URL", false);
        public static string ServicesAuthType => McmaEnvironmentVariables.Get("SERVICES_AUTH_TYPE", false);
        public static string ServicesAuthContext => McmaEnvironmentVariables.Get("SERVICES_AUTH_CONTEXT", false);
    }
}