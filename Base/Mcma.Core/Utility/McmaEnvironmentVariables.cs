using System;

namespace Mcma.Utility
{
    public static class McmaEnvironmentVariables
    {
        public const string Prefix = "MCMA_";

        public static string Get(string key, bool required = true)
        {
            var value = Environment.GetEnvironmentVariable(Prefix + key);
            
            if (value == null && required)
                throw new McmaException($"Required environment variable '{Prefix + key}' not set");

            return value;
        }
    }
}