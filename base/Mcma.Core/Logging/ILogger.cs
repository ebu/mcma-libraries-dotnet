namespace Mcma.Logging
{
    /// <summary>
    /// Interface for logging messages in MCMA
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a debug message (level = 500)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Logs a debug message (level = 500)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Debug(params object[] args);

        /// <summary>
        /// Logs an info message (level = 400)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Logs an info message (level = 400)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Info(params object[] args);

        /// <summary>
        /// Logs an warning message (level = 300)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// Logs an warning message (level = 300)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Warn(params object[] args);

        /// <summary>
        /// Logs an error message (level = 200)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Logs an error message (level = 200)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Error(params object[] args);

        /// <summary>
        /// Logs a fatal message (level = 100)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Logs a fatal message (level = 100)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void Fatal(params object[] args);

        /// <summary>
        /// Logs the start of an MCMA function (level = 450)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void FunctionStart(string message, params object[] args);

        /// <summary>
        /// Logs the end of an MCMA function (level = 450)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void FunctionEnd(string message, params object[] args);

        /// <summary>
        /// Logs the start of an MCMA job (level = 400)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void JobStart(string message, params object[] args);

        /// <summary>
        /// Logs the start of an MCMA job (level = 400)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void JobStart(params object[] args);

        /// <summary>
        /// Logs an update to an MCMA job (level = 400)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void JobUpdate(string message, params object[] args);

        /// <summary>
        /// Logs an update to an MCMA job (level = 400)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void JobUpdate(params object[] args);

        /// <summary>
        /// Logs the end of an MCMA job (level = 400)
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="args">A collection of arguments associated with the message</param>
        void JobEnd(string message, params object[] args);

        /// <summary>
        /// Logs the end of an MCMA job (level = 400)
        /// </summary>
        /// <param name="args">A collection of arguments associated with the message</param>
        void JobEnd(params object[] args);
    }
}