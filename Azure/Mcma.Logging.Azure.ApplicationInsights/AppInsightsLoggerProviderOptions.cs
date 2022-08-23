using System;
using Microsoft.ApplicationInsights.Extensibility;

namespace Mcma.Logging.Azure.ApplicationInsights;

public class AppInsightsLoggerProviderOptions : LoggerProviderOptions
{
    public TelemetryConfiguration TelemetryConfiguration { get; set; } = new()
    {
        ConnectionString = $"InstrumentationKey={Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")}"
    };
}