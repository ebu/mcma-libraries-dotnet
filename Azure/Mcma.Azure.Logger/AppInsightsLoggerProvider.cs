using Mcma.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Mcma.Azure.Logger
{
    public class AppInsightsLoggerProvider : LoggerProvider<AppInsightsLogger>
    {
        public AppInsightsLoggerProvider(string source, TelemetryClient telemetryClient = null)
            : base(source)
        {
            TelemetryClient = telemetryClient ?? new TelemetryClient(TelemetryConfiguration.CreateDefault());
        }

        private TelemetryClient TelemetryClient { get; }

        protected override AppInsightsLogger Get(string source, string requestId, McmaTracker tracker)
            => new AppInsightsLogger(TelemetryClient, source, requestId, tracker);

        public void Flush() => TelemetryClient.Flush();
    }
}