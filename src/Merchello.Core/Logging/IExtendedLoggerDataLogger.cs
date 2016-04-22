namespace Merchello.Core.Logging
{
    using System;

    public interface IExtendedLoggerDataLogger
    {


        /// <summary>
        /// The error.
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
        /// <param name="loggerData">
        /// Additional logger data
        /// </param>
        void Error(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData);

        /// <summary>
        /// Logs a warning .
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="loggerData">
        /// The logger data.
        /// </param>
        void Warn(Type callingType, string message, IExtendedLoggerData loggerData);

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
        /// <param name="loggerData">
        /// The logger data.
        /// </param>
        void WarnWithException(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData);

        /// <summary>
        /// Logs an informative message with extra logging data.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="loggerData">
        /// The logger data.
        /// </param>
        void Info(Type callingType, string message, IExtendedLoggerData loggerData);

        /// <summary>
        /// Logs a debug message with extra logging data.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="loggerData">
        /// The logger data.
        /// </param>
        void Debug(Type callingType, string message, IExtendedLoggerData loggerData);
    }
}