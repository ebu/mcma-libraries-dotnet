using System;
using Mcma.Model;
using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Logging;

/// <summary>
/// Represents a event to be logged
/// </summary>
public class LogEvent
{
    /// <summary>
    /// Instantiates a <see cref="LogEvent"/> with all required properties
    /// </summary>
    /// <param name="type">The type of log event (e.g. Debug, Info, Warn, etc). See <see cref="LogType"/> for a list of well-known types.</param>
    /// <param name="level">The log level of the event. Higher numbers are less severe. See <see cref="LogLevel"/> for a list of well-known levels.</param>
    /// <param name="source">The source from which this log event came</param>
    /// <param name="requestId"></param>
    /// <param name="timestamp"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <param name="tracker"></param>
    public LogEvent(
        string type,
        int level,
        string source,
        string requestId,
        DateTimeOffset timestamp, 
        string message,
        object[] args,
        McmaTracker tracker = null)
    {
        if (level <= 0) throw new ArgumentOutOfRangeException(nameof(level));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Level = level;
        Source = source ?? throw new ArgumentNullException(nameof(source));
        RequestId = requestId;
        Timestamp = timestamp != default ? timestamp : DateTimeOffset.UtcNow; 
        Message = message;
        Args = args;
        Tracker = tracker;
    }
        
    /// <summary>
    /// Gets the type of log event. See <see cref="LogType"/> for a list of well-known types for all MCMA services.
    /// </summary>
    public string Type { get; }
        
    /// <summary>
    /// Gets the log level of the event. See <see cref="LogLevel"/> for a list of well-defined values for all MCMA services.
    /// </summary>
    public int Level  { get; }
        
    /// <summary>
    /// Gets the source from which this log event came
    /// </summary>
    public string Source  { get; }
        
    /// <summary>
    /// Get the ID of the request being processed that generated this log event, if any
    /// </summary>
    public string RequestId { get; }
        
    /// <summary>
    /// Gets the date and time at which the log event occurred
    /// </summary>
    public DateTimeOffset Timestamp  { get; }
        
    /// <summary>
    /// Gets the message to be logged
    /// </summary>
    public string Message { get; }
        
    /// <summary>
    /// Gets the args associated with the event
    /// </summary>
    public object[] Args { get; }
        
    /// <summary>
    /// Gets the tracker for the MCMA operation that generated this event, if any
    /// </summary>
    public McmaTracker Tracker { get; }

    /// <summary>
    /// Flattens the log event by creating a simple key-value pair json object 
    /// </summary>
    /// <returns></returns>
    public JObject Flatten()
    {
        var json = (JObject)this.ToMcmaJson();

        var trackerProperty = json.Property(nameof(Tracker), StringComparison.OrdinalIgnoreCase);
        trackerProperty?.Remove();

        if (Tracker != null)
        {
            json["trackerId"] = Tracker.Id;
            json["trackerLabel"] = Tracker.Label;

            if (Tracker.Custom != null)
                foreach (var customProperty in Tracker.Custom)
                {
                    var customPropertyKey =
                        nameof(Tracker.Id).Equals(customProperty.Key, StringComparison.OrdinalIgnoreCase) || 
                        nameof(Tracker.Label).Equals(customProperty.Key, StringComparison.OrdinalIgnoreCase)
                            ? $"trackerCustom{customProperty.Key}"
                            : $"tracker{customProperty.Key}";

                    json[customPropertyKey] = customProperty.Value;
                }
        }

        return json;
    }

    /// <summary>
    /// Gets the string representation of the event by flattening it using <see cref="Flatten"/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Flatten().ToString();
}