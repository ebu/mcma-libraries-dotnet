using Mcma.Logging;
using Microsoft.ApplicationInsights.Extensibility;

namespace Mcma.Azure.Logger
{
    public class AppInsightsLoggerProviderOptions : LoggerProviderOptions
    {
        public TelemetryConfiguration TelemetryConfiguration { get; set; }
    }
}