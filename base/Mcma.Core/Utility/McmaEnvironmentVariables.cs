using System;

namespace Mcma.Utility
{
    public static class McmaEnvironmentVariables
    {
        public const string Prefix = "MCMA_";

        public static string Get(string key) => Environment.GetEnvironmentVariable(Prefix + key);
    }
}