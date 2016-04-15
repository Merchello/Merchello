namespace Merchello.Core.Logging
{
    using System;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a remote logging provider.
    /// </summary>
    /// <remarks>
    /// Allows for developers to wire in external logging services
    /// This is designated as a service due to legacy reasons.
    /// </remarks>
    public interface IRemoteLogger : IExtendedLoggerDataLogger, IService
    {
        /// <summary>
        /// Gets a value indicating whether the provider is ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Logs an error with format items.
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
        void Error(Type callingType, string message, Exception exception);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void Warn(Type callingType, string message);

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
        void WarnWithException(Type callingType, string message, Exception exception);

        /// <summary>
        /// Logs an informative message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void Info(Type callingType, string message);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void Debug(Type callingType, string message);
    }
}