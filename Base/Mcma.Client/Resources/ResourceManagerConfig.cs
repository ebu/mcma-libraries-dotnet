using System;

namespace Mcma.Client
{
    public class ResourceManagerConfig
    {
        public ResourceManagerConfig(string servicesUrl, string servicesAuthType = null, string servicesAuthContext = null)
        {
            ServicesUrl = servicesUrl ?? throw new ArgumentNullException(nameof(servicesUrl));
            ServicesAuthType = servicesAuthType;
            ServicesAuthContext = servicesAuthContext;
        }

        public string ServicesUrl { get; }

        public string ServicesAuthType { get; set; }

        public string ServicesAuthContext { get; set; }
    }
}