using System.Threading.Tasks;
using Mcma.Model;

namespace Mcma.Logging
{
    /// <summary>
    /// Interface for getting <see cref="ILogger"/> for logging in MCMA
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// Gets an <see cref="ILogger"/>
        /// </summary>
        /// <param name="requestId">The ID of the current request, if any. This is generally a unique ID provided by the platform on which the code is running.</param>
        /// <param name="tracker">The tracker for the current MCMA operation, if any</param>
        /// <returns>An <see cref="ILogger"/> for the current request and tracker</returns>
        ILogger Get(string requestId = null, McmaTracker tracker = null);

        /// <summary>
        /// Flushes any buffering of logs that might be done on the client side. This may or may not be necessary for all loggers.
        /// </summary>
        /// <returns>A task that completes when all log messages have been flushed</returns>
        Task FlushAsync();
    }
}