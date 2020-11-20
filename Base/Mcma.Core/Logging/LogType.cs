namespace Mcma.Logging
{
    /// <summary>
    /// Contains well-known log types
    /// </summary>
    /// <remarks>Log types in MCMA are labels to generally indicate why a log message is being written. Some of these map 1-to-1 with log levels, but they
    /// are not required to, and custom values can be used, as they are just strings.</remarks>
    public static class LogType
    {
        /// <summary>
        /// Type for general <see cref="LogLevel.Fatal"/> messages 
        /// </summary>
        public const string Fatal = "FATAL";
        
        /// <summary>
        /// Type for general <see cref="LogLevel.Error"/> messages 
        /// </summary>
        public const string Error = "ERROR";
        
        /// <summary>
        /// Type for general <see cref="LogLevel.Warn"/> messages 
        /// </summary>
        public const string Warn = "WARN";
        
        /// <summary>
        /// Type for general <see cref="LogLevel.Info"/> messages 
        /// </summary>
        public const string Info = "INFO";
        
        /// <summary>
        /// Type for general <see cref="LogLevel.Debug"/> messages 
        /// </summary>
        public const string Debug = "DEBUG";
        
        internal const string FunctionStart = "FUNCTION_START";
        internal const string FunctionEnd = "FUNCTION_END";
        internal const string JobStart = "JOB_START";
        internal const string JobUpdate = "JOB_UPDATE";
        internal const string JobEnd = "JOB_END";
    }
}