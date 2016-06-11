namespace Merchello.Web.Logging
{
    using System;

    using Merchello.Core.Logging;

    /// <summary>
    /// A default remote logger that does nothing.
    /// </summary>
    public class DefaultEmptyRemoteLogger : RemoteLoggerBase
    {
        /// <summary>
        /// Gets a value indicating whether is ready.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Does nothing.
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
        public override void Error(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData)
        {
            return;
        }

        /// <summary>
        /// Does nothing.
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
        public override void Warn(Type callingType, string message, IExtendedLoggerData loggerData)
        {
            return;
        }

        /// <summary>
        /// Does nothing.
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
        public override void WarnWithException(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData)
        {
            return;
        }

        /// <summary>
        /// Does nothing.
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
        public override void Info(Type callingType, string message, IExtendedLoggerData loggerData)
        {
            return;
        }

        /// <summary>
        /// Does nothing.
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
        public override void Debug(Type callingType, string message, IExtendedLoggerData loggerData)
        {
            return;
        }
    }
}