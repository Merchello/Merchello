namespace Merchello.Core.Logging
{
    using System;

    /// <summary>
    /// A base class for LoggerProviders.
    /// </summary>
    public abstract class RemoteLoggerBase : IRemoteLogger
    {
        /// <summary>
        /// Gets a value indicating whether is ready.
        /// </summary>
        public abstract bool IsReady { get; }

        /// <summary>
        /// Logs an error.
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
        public virtual void Error(Type callingType, string message, Exception exception)
        {
            Error(callingType, message, exception, new ExtendedLoggerData());
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void Warn(Type callingType, string message)
        {
            Warn(callingType, message, new ExtendedLoggerData());
        }

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
        public virtual void WarnWithException(Type callingType, string message, Exception exception)
        {
            WarnWithException(callingType, message, exception, new ExtendedLoggerData());
        }

        /// <summary>
        /// Logs an informative message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void Info(Type callingType, string message)
        {
            Info(callingType, message, new ExtendedLoggerData());
        }

        /// <summary>
        /// Logs debug information.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void Debug(Type callingType, string message)
        {
            Debug(callingType, message, new ExtendedLoggerData());
        }

        /// <summary>
        /// Logs an exception.
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
        public abstract void Error(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData);

        /// <summary>
        /// Logs a warning.
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
        public abstract void Warn(Type callingType, string message, IExtendedLoggerData loggerData);

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
        public abstract void WarnWithException(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData);

        /// <summary>
        /// Logs an informative message.
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
        public abstract void Info(Type callingType, string message, IExtendedLoggerData loggerData);

        /// <summary>
        /// Logs debug information.
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
        public abstract void Debug(Type callingType, string message, IExtendedLoggerData loggerData);
    }
}