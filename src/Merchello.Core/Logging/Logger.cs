namespace Merchello.Core.Logging
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Web;

    using log4net;
    using log4net.Config;

    /// <summary>
    /// Represents logging service.
    /// </summary>
    /// <remarks>
    /// Class ported to Merchello core so that we can reduce the number of dependencies on the UmbracoCms.Core package.
    /// </remarks>
    /// UMBRACO_SRC
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Logging/Logger.cs"/>
    public class Logger : ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="log4NetConfigFile">
        /// The log 4 net config file.
        /// </param>
        public Logger(FileInfo log4NetConfigFile)
           : this()
        {
            XmlConfigurator.Configure(log4NetConfigFile);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Logger"/> class from being created.
        /// </summary>
        private Logger()
        {
            //// Add custom global properties to the log4net context that we can use in our logging output

            log4net.GlobalContext.Properties["processId"] = Process.GetCurrentProcess().Id;
            log4net.GlobalContext.Properties["appDomainId"] = AppDomain.CurrentDomain.Id;
        }

        /// <summary>
        /// Creates a logger with the default log4net configuration discovered (i.e. from the web.config)
        /// </summary>
        /// <returns>A new instance of the logger</returns>
        public static Logger CreateWithDefaultLog4NetConfiguration()
        {
            return new Logger();
        }

        /// <inheritdoc/>
        public void Error(Type callingType, string message, Exception exception)
        {
            var logger = LogManager.GetLogger(callingType);
            logger?.Error((message), exception);
        }

        /// <inheritdoc/>
        public void Warn(Type callingType, string message, params Func<object>[] formatItems)
        {
            var logger = LogManager.GetLogger(callingType);
            if (logger == null || logger.IsWarnEnabled == false) return;
            logger.WarnFormat((message), formatItems.Select(x => x.Invoke()).ToArray());
        }

        /// <inheritdoc/>
        public void Warn(Type callingType, string message, bool showHttpTrace, params Func<object>[] formatItems)
        {
            Ensure.ParameterNotNull(callingType, "callingType");
            Ensure.ParameterNotNullOrEmpty(message, "message");

            if (showHttpTrace && HttpContext.Current != null)
            {
                HttpContext.Current.Trace.Warn(callingType.Name, string.Format(message, formatItems.Select(x => x.Invoke()).ToArray()));
            }

            var logger = LogManager.GetLogger(callingType);
            if (logger == null || logger.IsWarnEnabled == false) return;
            logger.WarnFormat(message, formatItems.Select(x => x.Invoke()).ToArray());

        }

        /// <inheritdoc/>
        public void WarnWithException(Type callingType, string message, Exception e, params Func<object>[] formatItems)
        {
            Ensure.ParameterNotNull(e, "e");
            Ensure.ParameterNotNull(callingType, "callingType");
            Ensure.ParameterNotNullOrEmpty(message, "message");

            var logger = LogManager.GetLogger(callingType);
            if (logger == null || logger.IsWarnEnabled == false) return;
            var executedParams = formatItems.Select(x => x.Invoke()).ToArray();
            logger.WarnFormat((message) + ". Exception: " + e, executedParams);
        }

        /// <inheritdoc/>
        public void Info(Type callingType, Func<string> generateMessage)
        {
            var logger = LogManager.GetLogger(callingType);
            if (logger == null || logger.IsInfoEnabled == false) return;
            logger.Info((generateMessage.Invoke()));
        }

        /// <inheritdoc/>
        public void Info(Type type, string generateMessageFormat, params Func<object>[] formatItems)
        {
            var logger = LogManager.GetLogger(type);
            if (logger == null || logger.IsInfoEnabled == false) return;
            var executedParams = formatItems.Select(x => x.Invoke()).ToArray();
            logger.InfoFormat((generateMessageFormat), executedParams);
        }


        /// <inheritdoc/>
        public void Debug(Type callingType, Func<string> generateMessage)
        {
            var logger = LogManager.GetLogger(callingType);
            if (logger == null || logger.IsDebugEnabled == false) return;
            logger.Debug((generateMessage.Invoke()));
        }

        /// <inheritdoc/>
        public void Debug(Type type, string generateMessageFormat, params Func<object>[] formatItems)
        {
            var logger = LogManager.GetLogger(type);
            if (logger == null || logger.IsDebugEnabled == false) return;
            var executedParams = formatItems.Select(x => x.Invoke()).ToArray();
            logger.DebugFormat((generateMessageFormat), executedParams);
        }
    }
}