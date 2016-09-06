namespace Merchello.Core.Logging
{
    using System;

    /// <summary>
    /// Defines a logging service.
    /// </summary>
    /// REFACTOR - When code port has been completed, rename this interface to avoid naming ambiguity issues in IDE
    /// UMBRACO_SRC
    public interface ILogger
    {
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The error message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void Error(Type callingType, string message, Exception exception);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The warning message.
        /// </param>
        /// <param name="formatItems">
        /// A collection of formatted items to be logged.
        /// </param>
        void Warn(Type callingType, string message, params Func<object>[] formatItems);

        /// <summary>
        /// Logs a warning with an exception.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="formatItems">
        /// The collection of methods that provide formatted items to be logged.
        /// </param>
        void WarnWithException(Type callingType, string message, Exception exception, params Func<object>[] formatItems);

        /// <summary>
        /// Traces a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="generateMessage">
        /// The method that generates the message.
        /// </param>
        void Info(Type callingType, Func<string> generateMessage);

        /// <summary>
        /// Traces a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The formatted message.
        /// </param>
        /// <param name="formatItems">
        /// A collection of delegates to generate a messages.
        /// </param>
        void Info(Type callingType, string message, params Func<object>[] formatItems);

        /// <summary>
        /// Debugs a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="generateMessage">
        /// The delegate that generates the message.
        /// </param>
        void Debug(Type callingType, Func<string> generateMessage);

        /// <summary>
        /// Debugs a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="generateMessageFormat">
        /// The delegate that generates the message.
        /// </param>
        /// <param name="formatItems">
        /// A collection of delegates to generate a messages.
        /// </param>
        void Debug(Type callingType, string generateMessageFormat, params Func<object>[] formatItems);
    }
}