namespace Merchello.Core.Logging
{
    using Umbraco.Core.Logging;

    /// <summary>
    /// Marker interface for the a MultiLogger.
    /// </summary>
    public interface IMultiLogger : ILogger, IExtendedLoggerDataLogger
    {
        /// <summary>
        /// Gets the Umbraco <see cref="ILogger"/>.
        /// </summary>
        ILogger UmbracoLogger { get; }

        /// <summary>
        /// Gets the remote logger.
        /// </summary>
        IRemoteLogger RemoteLogger { get; }
    }
}