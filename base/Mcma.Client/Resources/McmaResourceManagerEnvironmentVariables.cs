using Mcma.Utility;

namespace Mcma.Client
{
    public static class McmaResourceManagerEnvironmentVariables
    {
        public static string ServicesUrl => McmaEnvironmentVariables.Get("SERVICES_URL");
        public static string ServicesAuthType => McmaEnvironmentVariables.Get("SERVICES_AUTH_TYPE");
        public static string ServicesAuthContext => McmaEnvironmentVariables.Get("SERVICES_AUTH_CONTEXT", false);
    }
}