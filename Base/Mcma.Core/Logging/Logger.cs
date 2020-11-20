using System;

namespace Mcma.Logging
{
    /// <summary>
    /// Abstract base for implementing loggers. Simplifies implementations by preventing the need to write separate methods for each level (e.g. Debug, Info, Warn, etc)
    /// </summary>
    public abstract class Logger : ILogger
    {
        /// <summary>
        /// Instantiates a <see cref="Logger"/>
        /// </summary>
        /// <param name="source">The source from which the log messages are coming</param>
        /// <param name="requestId">The ID of the current request, if any. This is generally a unique ID provided by the platform on which the code is running.</param>
        /// <param name="tracker">The tracker for the current MCMA operation, if any</param>
        protected Logger(string source, string requestId, McmaTracker tracker)
        {
            Source = source;
            RequestId = requestId ?? Guid.NewGuid().ToString();
            Tracker = tracker;
        }

        /// <summary>
        /// Gets the source from which the log messages are coming
        /// </summary>
        protected string Source { get; }

        /// <summary>
        /// Gets the ID of the current request, if any
        /// </summary>
        protected string RequestId { get; }

        /// <summary>
        /// Gets the tracker for the current MCMA operation, if any
        /// </summary>
        protected McmaTracker Tracker { get; }

        /// <summary>
        /// Gets or sets a statically-available fallback logger. Defaults to using <see cref="ConsoleLogger"/>
        /// </summary>
        public static ILogger System { get; set; } = new ConsoleLogger(nameof(System));

        /// <summary>
        /// Builds a <see cref="LogEvent"/> with a given level, type, message, and args
        /// </summary>
        /// <param name="level"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected LogEvent BuildLogEvent(int level, string type, string message, object[] args)
            => new LogEvent(type, level, Source, RequestId, DateTimeOffset.UtcNow, message, args, Tracker);

        /// <summary>
        /// To be overridden by implementers defining where log messages are written
        /// </summary>
        /// <param name="logEvent"></param>
        protected abstract void WriteLogEvent(LogEvent logEvent);
        
        private void Log(int level, string type, string message, params object[] args)
            => WriteLogEvent(BuildLogEvent(level, type, message, args));

        /// <inheritdoc />
        public void Fatal(string message, params object[] args)
            => Log(LogLevel.Fatal, LogType.Fatal, message, args);
        /// <inheritdoc />
        public void Fatal(params object[] args)
            => Log(LogLevel.Fatal, LogType.Fatal, null, args);
        
        /// <inheritdoc />
        public void Error(string message, params object[] args)
            => Log(LogLevel.Error, LogType.Error, message, args);
        /// <inheritdoc />
        public void Error(params object[] args)
            => Log(LogLevel.Error, LogType.Error, null, args);
        
        /// <inheritdoc />
        public void Warn(string message, params object[] args)
            => Log(LogLevel.Warn, LogType.Warn, message, args);
        /// <inheritdoc />
        public void Warn(params object[] args)
            => Log(LogLevel.Warn, LogType.Warn, null, args);
        
        /// <inheritdoc />
        public void Info(string message, params object[] args)
            => Log(LogLevel.Info, LogType.Info, message, args);
        /// <inheritdoc />
        public void Info(params object[] args)
            => Log(LogLevel.Info, LogType.Info, null, args);

        /// <inheritdoc />
        public void Debug(string message, params object[] args)
            => Log(LogLevel.Debug, LogType.Debug, message, args);
        /// <inheritdoc />
        public void Debug(params object[] args)
            => Log(LogLevel.Debug, LogType.Debug, null, args);

        /// <inheritdoc />
        public void FunctionStart(string message, params object[] args)
            => Log(LogLevel.FunctionEvent, LogType.FunctionStart, message, args);
        /// <inheritdoc />
        public void FunctionEnd(string message, params object[] args)
            => Log(LogLevel.FunctionEvent, LogType.FunctionEnd, message, args);

        /// <inheritdoc />
        public void JobStart(params object[] args)
            => Log(LogLevel.Info, LogType.JobStart, null, args);
        /// <inheritdoc />
        public void JobStart(string message, params object[] args)
            => Log(LogLevel.Info, LogType.JobStart, message, args);
        /// <inheritdoc />
        public void JobUpdate(params object[] args)
            => Log(LogLevel.Info, LogType.JobUpdate, null, args);
        /// <inheritdoc />
        public void JobUpdate(string message, params object[] args)
            => Log(LogLevel.Info, LogType.JobUpdate, message, args);
        /// <inheritdoc />
        public void JobEnd(params object[] args)
            => Log(LogLevel.Info, LogType.JobEnd, null, args);
        /// <inheritdoc />
        public void JobEnd(string message, params object[] args)
            => Log(LogLevel.Info, LogType.JobEnd, message, args);
    }
}