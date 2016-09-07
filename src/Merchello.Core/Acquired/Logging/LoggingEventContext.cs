namespace Merchello.Core.Logging
{
    using log4net.Core;

    /// <remarks>
    /// Borrowed from https://github.com/cjbhaines/Log4Net.Async - will reference Nuget packages directly in v8
    /// </remarks>
    /// UMBRACO_SRC
    internal sealed class LoggingEventContext
    {
        public LoggingEventContext(LoggingEvent loggingEvent, object httpContext)
        {
            this.LoggingEvent = loggingEvent;
            this.HttpContext = httpContext;
        }

        public LoggingEvent LoggingEvent { get; set; }

        public object HttpContext { get; set; }
    }
}