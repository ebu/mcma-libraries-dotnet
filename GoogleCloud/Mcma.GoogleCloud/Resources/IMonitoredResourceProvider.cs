﻿using System.Threading.Tasks;
using Google.Api;

namespace Mcma.GoogleCloud.Resources
{
    public interface IMonitoredResourceProvider
    {
        Task<MonitoredResource> GetCurrentResourceAsync();
    }
}