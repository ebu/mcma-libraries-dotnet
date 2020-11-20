namespace Mcma.Logging
{
    /// <summary>
    /// Contains well-known log levels
    /// </summary>
    /// <remarks>Log levels in MCMA are just integers and allow for more granular refinement or customization than the well-known values. This class
    /// is just intended to provide labels to the well-known values</remarks>
    public static class LogLevel
    {
        /// <summary>
        /// Used when an error crashes or destabilizes the service 
        /// </summary>
        public const int Fatal = 100;
        
        /// <summary>
        /// Used when an expected or handled error occurs
        /// </summary>
        public const int Error = 200;
        
        /// <summary>
        /// Used when something should be flagged as potentially problematic but it is still possible to proceed
        /// </summary>
        public const int Warn = 300;
        
        /// <summary>
        /// Used when logging high-level informational messages
        /// </summary>
        public const int Info = 400;
        
        /// <summary>
        /// Used when logging trace-level technical messages
        /// </summary>
        public const int Debug = 500;
        
        internal const int FunctionEvent = 450;
    }
}