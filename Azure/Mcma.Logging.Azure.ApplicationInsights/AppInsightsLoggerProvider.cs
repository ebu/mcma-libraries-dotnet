using System.Threading.Tasks;
using Mcma.Model;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;

namespace Mcma.Logging.Azure.ApplicationInsights;

public class AppInsightsLoggerProvider : LoggerProvider<AppInsightsLogger>
{
    public AppInsightsLoggerProvider(IOptions<AppInsightsLoggerProviderOptions> options)
        : base(options)
    {
        TelemetryClient = new TelemetryClient(options.Value.TelemetryConfiguration ?? TelemetryConfiguration.CreateDefault());
    }

    private TelemetryClient TelemetryClient { get; }

    protected override AppInsightsLogger Get(string source, string requestId, McmaTracker tracker) => new(TelemetryClient, source, requestId, tracker);

    public override Task FlushAsync()
    {
        TelemetryClient.Flush();
        return Task.CompletedTask;
    }
}