namespace Merchello.Umbraco.Adapters.Logging
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Umbraco.Adapters;

    /// <summary>
    /// An adapter for using Umbraco's <see><cref>global::Umbraco.Core.Logging.ILogger</cref></see>
    /// as <see cref="ILogger"/>
    /// </summary>
    internal class LoggerAdapter : ILogger, IUmbracoAdapter
    {
        /// <summary>
        /// Umbraco's actual logger.
        /// </summary>
        private readonly global::Umbraco.Core.Logging.ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerAdapter"/> class.
        /// </summary>
        public LoggerAdapter()
            : this(global::Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerAdapter"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public LoggerAdapter(global::Umbraco.Core.Logging.ILogger logger)
        {
            Ensure.ParameterNotNull(logger, "Logger was null");
            this._logger = logger;
        }

        /// <inheritdoc/>
        public void Error(Type callingType, string message, Exception exception)
        {
            this._logger.Error(callingType, message, exception);
        }

        /// <inheritdoc/>
        public void Warn(Type callingType, string message, params Func<object>[] formatItems)
        {
            this._logger.Warn(callingType, message, formatItems);
        }

        /// <inheritdoc/>
        public void WarnWithException(Type callingType, string message, Exception exception, params Func<object>[] formatItems)
        {
            this._logger.WarnWithException(callingType, message, exception, formatItems);
        }

        /// <inheritdoc/>
        public void Info(Type callingType, Func<string> generateMessage)
        {
            this._logger.Info(callingType, generateMessage);
        }

        /// <inheritdoc/>
        public void Info(Type callingType, string message, params Func<object>[] formatItems)
        {
            this._logger.Info(callingType, message, formatItems);
        }

        /// <inheritdoc/>
        public void Debug(Type callingType, Func<string> generateMessage)
        {
            this._logger.Debug(callingType, generateMessage);
        }

        /// <inheritdoc/>
        public void Debug(Type callingType, string generateMessageFormat, params Func<object>[] formatItems)
        {
            this._logger.Debug(callingType, generateMessageFormat, formatItems);
        }
    }
}