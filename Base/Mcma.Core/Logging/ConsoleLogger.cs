using Mcma.Model;
using Newtonsoft.Json.Linq;

namespace Mcma.Logging;

/// <summary>
/// Represents an MCMA logger that writes messages using <see cref="Console"/>
/// </summary>
public class ConsoleLogger : Logger
{
    /// <summary>
    /// Instantiates a <see cref="ConsoleLogger"/> with a request ID and tracker
    /// </summary>
    /// <param name="source">The source from which the log messages are coming</param>
    /// <param name="requestId">The ID of the current request, if any</param>
    /// <param name="tracker">The tracker for the current MCMA operation, if any</param>
    public ConsoleLogger(string source, string? requestId = null, McmaTracker? tracker = null)
        : base(source, requestId, tracker)
    {
    }

    /// <summary>
    /// Writes a log event to the console, using colored text based on the log level of the event
    /// </summary>
    /// <param name="logEvent">The log event to write to the console</param>
    protected override void WriteLogEvent(LogEvent logEvent)
    {
        if (logEvent.Level <= 0)
            return;

        if (logEvent.Level < 200)
            WriteToConsole(logEvent, ConsoleColor.Red);
        else if (logEvent.Level < 300)
            WriteToConsole(logEvent, ConsoleColor.Yellow);
        else
            WriteToConsole(logEvent);
    }

    /// <summary>
    /// Writes a log event to the console with a given color
    /// </summary>
    /// <param name="logEvent"></param>
    /// <param name="color"></param>
    private static void WriteToConsole(LogEvent logEvent, ConsoleColor? color = null)
    {
        var origColor = Console.ForegroundColor;
        try
        {
            if (color.HasValue)
                Console.ForegroundColor = color.Value;

            var message =
                string.Join("|",
                            logEvent.Timestamp.ToString("yyyy-MM-ddThh:mm:ss.fff"),
                            logEvent.Level,
                            logEvent.Source,
                            logEvent.Type,
                            logEvent.Tracker?.Id ?? "(empty)",
                            logEvent.Tracker?.Label ?? "(empty)",
                            logEvent.Message ?? "(empty)",
                            logEvent.Args != null ? JArray.FromObject(logEvent.Args).ToString() : "(empty)");

            Console.WriteLine(message);
        }
        finally
        {
            Console.ForegroundColor = origColor;
        }
    }
}