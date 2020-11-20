﻿using Mcma.Utility;

namespace Mcma.Api
{
    public static class McmaApiEnvironmentVariables
    {
        public static string PublicUrl => McmaEnvironmentVariables.Get("PUBLIC_URL", false);
    }
}