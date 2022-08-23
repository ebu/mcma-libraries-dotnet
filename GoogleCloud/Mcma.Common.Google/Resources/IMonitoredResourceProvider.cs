using System.Threading.Tasks;
using Google.Api;

namespace Mcma.Common.Google.Resources;

public interface IMonitoredResourceProvider
{
    Task<MonitoredResource> GetCurrentResourceAsync();
}