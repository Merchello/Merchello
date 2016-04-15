namespace Merchello.Core.Logging
{
    using Umbraco.Core.Logging;

    /// <summary>
    /// Marker interface for the a MultiLogger.
    /// </summary>
    public interface IMultiLogger : ILogger, IExtendedLoggerDataLogger
    {
    }
}