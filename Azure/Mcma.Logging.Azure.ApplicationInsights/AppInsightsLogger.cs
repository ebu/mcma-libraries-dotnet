using System;
using System.Globalization;
using Mcma.Model;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json.Linq;

namespace Mcma.Logging.Azure.ApplicationInsights;

public class AppInsightsLogger : Logger
{
    public AppInsightsLogger(TelemetryClient telemetryClient, string source, string requestId = null, McmaTracker tracker = null)
        : base(source, requestId, tracker)
    {
        TelemetryClient = telemetryClient;
    }

    private TelemetryClient TelemetryClient { get; }

    protected override void WriteLogEvent(LogEvent logEvent)
    {
        TelemetryClient.TrackTrace(CreateTraceTelemetry(logEvent));
    }

    private static TraceTelemetry CreateTraceTelemetry(LogEvent logEvent)
    {
        var message = logEvent.Message;
        var flattenedLogEvent = logEvent.Flatten();
            
        var traceTelemetry = new TraceTelemetry(message, GetSeverityLevel(logEvent.Level)) {Timestamp = logEvent.Timestamp};

        foreach (var property in flattenedLogEvent.Properties())
            traceTelemetry.Properties[property.Name] = GetStringValue(property.Value);

        return traceTelemetry;
    }

    private static SeverityLevel GetSeverityLevel(int logLevel)
        => logLevel switch
        {
            0 => SeverityLevel.Verbose,
            var x when x <= LogLevel.Fatal => SeverityLevel.Critical,
            var x when x <= LogLevel.Error => SeverityLevel.Error,
            var x when x <= LogLevel.Warn => SeverityLevel.Warning,
            var x when x <= LogLevel.Info => SeverityLevel.Information,
            _ => SeverityLevel.Verbose
        };

    private static string GetStringValue(JToken value)
        => value.Type switch
        {
            JTokenType.Null => null,
            JTokenType.String => value.Value<string>(),
            JTokenType.Integer => value.Value<long>().ToString(),
            JTokenType.Float => value.Value<decimal>().ToString(CultureInfo.InvariantCulture),
            JTokenType.Boolean => value.Value<bool>().ToString(),
            JTokenType.Date => value.Value<DateTimeOffset>().ToString("O"),
            _ => value.ToString()
        };
}