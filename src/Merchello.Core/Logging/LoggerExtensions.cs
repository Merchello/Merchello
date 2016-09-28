namespace Merchello.Core.Logging
{
    using System;

    /// <summary>
    /// Extension methods of <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        public static void Error<T>(this ILogger logger, string message, Exception exception)
        {
            logger.Error(typeof(T), message, exception);
        }

        /// <summary>
        /// Logs a warning log
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="formatItems">
        /// A collection of delegates to generate a messages.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        public static void Warn<T>(this ILogger logger, string message, params Func<object>[] formatItems)
        {
            logger.Warn(typeof(T), message, formatItems);
        }

        /// <summary>
        /// Logs a warning with exception.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="formatItems">
        /// A collection of delegates to generate a messages.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        public static void WarnWithException<T>(this ILogger logger, string message, Exception exception, params Func<object>[] formatItems)
        {
            logger.WarnWithException(typeof(T), message, exception, formatItems);
        }


        /// <summary>
        /// Traces a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="formatItems">
        /// A collection of delegates to generate a messages to be appended.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        public static void Info<T>(this ILogger logger, string message, params Func<object>[] formatItems)
        {
            logger.Info(typeof(T), message, formatItems);
        }

        /// <summary>
        /// Traces a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="generateMessage">
        /// The delegate that generates the message.
        /// </param>
        public static void Info<T>(this ILogger logger, Func<string> generateMessage)
        {
            logger.Info(typeof(T), generateMessage);
        }

        /// <summary>
        /// Debugs a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="generateMessage">
        /// The delegate that generates the message.
        /// </param>
        public static void Debug<T>(this ILogger logger, Func<string> generateMessage)
        {
            logger.Debug(typeof(T), generateMessage);
        }

        /// <summary>
        /// Debugs a message, only generating the message if debug is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="formatItems">
        /// A collection of delegates to generate a messages to be appended.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        public static void Debug<T>(this ILogger logger, string message, params Func<object>[] formatItems)
        {
            logger.Debug(typeof(T), message, formatItems);
        }
    }
}