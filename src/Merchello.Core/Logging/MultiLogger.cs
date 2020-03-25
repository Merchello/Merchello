namespace Merchello.Core.Logging
{
    using System;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// A logger capable of logging to multiple sources.
    /// </summary>
    /// <remarks>
    /// This is a wrapper for Umbraco's ILogger
    /// </remarks>
    public class MultiLogger : IMultiLogger
    {
        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _umbracoLogger;

        /// <summary>
        /// The <see cref="IRemoteLogger"/>.
        /// </summary>
        private readonly IRemoteLogger _remoteLogger;

        /// <summary>
        /// A value indicating whether or not a remote logger is configured.
        /// </summary>
        private readonly bool _hasRemoteLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLogger"/> class.
        /// </summary>
        public MultiLogger()
            : this(LoggerResolver.HasCurrent == false || LoggerResolver.Current.HasValue == false ? Logger.CreateWithDefaultLog4NetConfiguration() : LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MultiLogger(ILogger logger)
            : this(logger, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="remoteRemoteLogger">
        /// The remote logger.
        /// </param>
        public MultiLogger(ILogger logger, IRemoteLogger remoteRemoteLogger)
            : base()
        {
            Mandate.ParameterNotNull(logger, "logger");
            _umbracoLogger = logger;

            if (remoteRemoteLogger != null)
            {
                this._remoteLogger = remoteRemoteLogger;
                _hasRemoteLogger = true;
            }
            else
            {
                _hasRemoteLogger = false;
            }

        }

        /// <summary>
        /// Gets the Umbraco <see cref="ILogger"/>.
        /// </summary>
        public virtual ILogger UmbracoLogger
        {
            get
            {
                return _umbracoLogger;
            }
        }

        /// <summary>
        /// Gets the <see cref="IRemoteLogger"/>.
        /// </summary>
        public virtual IRemoteLogger RemoteLogger
        {
            get
            {
                return _remoteLogger;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is ready.
        /// </summary>
        public virtual bool IsReady
        {
            get
            {
                return true;
            }
        }

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
        public void Error(Type callingType, string message, Exception exception)
        {
            _umbracoLogger.Error(callingType, message, exception);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Error(callingType, message, exception, GetBaseLoggingData());
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
        /// <param name="formatItems">
        /// The format items.
        /// </param>
        public void Warn(Type callingType, string message, params Func<object>[] formatItems)
        {
            _umbracoLogger.Warn(callingType, message, formatItems);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Warn(callingType, message, GetBaseLoggingData());
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
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="formatItems">
        /// The format items.
        /// </param>
        public void WarnWithException(Type callingType, string message, Exception e, params Func<object>[] formatItems)
        {
            _umbracoLogger.WarnWithException(callingType, message, e, formatItems);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.WarnWithException(callingType, message, e, GetBaseLoggingData());
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="generateMessage">
        /// The generate message.
        /// </param>
        public void Info(Type callingType, Func<string> generateMessage)
        {
            _umbracoLogger.Info(callingType, generateMessage);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Info(callingType, generateMessage.Invoke(), GetBaseLoggingData());
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="generateMessageFormat">
        /// The generate message format.
        /// </param>
        /// <param name="formatItems">
        /// The format items.
        /// </param>
        public void Info(Type type, string generateMessageFormat, params Func<object>[] formatItems)
        {
            _umbracoLogger.Info(type, generateMessageFormat, formatItems);
        }

        /// <summary>
        /// Logs debug information.
        /// </summary>
        /// <param name="callingType">
        /// The calling type.
        /// </param>
        /// <param name="generateMessage">
        /// The generate message.
        /// </param>
        public void Debug(Type callingType, Func<string> generateMessage)
        {
            _umbracoLogger.Debug(callingType, generateMessage);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Debug(callingType, generateMessage.Invoke(), GetBaseLoggingData());
        }

        /// <summary>
        /// Logs debug information.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="generateMessageFormat">
        /// The generate message format.
        /// </param>
        /// <param name="formatItems">
        /// The format items.
        /// </param>
        public void Debug(Type type, string generateMessageFormat, params Func<object>[] formatItems)
        {
            _umbracoLogger.Debug(type, generateMessageFormat, formatItems);
        }

        /// <summary>
        /// Logs an error with extra log data.
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
        public void Error(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData)
        {
            _umbracoLogger.Error(callingType, message, exception);

            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Error(callingType, message, exception, loggerData);
        }

        /// <summary>
        /// Logs a warning with extra log data.
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
        public void Warn(Type callingType, string message, IExtendedLoggerData loggerData)
        {
            _umbracoLogger.Warn(callingType, message);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Warn(callingType, message, loggerData);
        }

        /// <summary>
        /// Warns with an exception.
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
        public void WarnWithException(Type callingType, string message, Exception exception, IExtendedLoggerData loggerData)
        {
            _umbracoLogger.WarnWithException(callingType, message, exception);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.WarnWithException(callingType, message, exception);
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
        /// <param name="loggerData">
        /// The logger data.
        /// </param>
        public void Info(Type callingType, string message, IExtendedLoggerData loggerData)
        {
            _umbracoLogger.Info(callingType, () => message);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Info(callingType, message, loggerData);
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
        /// <param name="loggerData">
        /// The logger data.
        /// </param>
        public void Debug(Type callingType, string message, IExtendedLoggerData loggerData)
        {
            _umbracoLogger.Debug(callingType, () => message);
            if (_hasRemoteLogger && this._remoteLogger.IsReady) _remoteLogger.Debug(callingType, message, loggerData);
        }

        /// <summary>
        /// The get base logging data.
        /// </summary>
        /// <returns>
        /// The <see cref="IExtendedLoggerData"/>.
        /// </returns>
        public static IExtendedLoggerData GetBaseLoggingData()
        {
            var data = new ExtendedLoggerData();
            data.AddCategory("Merchello");

            return data;
        }
    }
}