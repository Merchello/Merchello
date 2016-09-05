namespace Merchello.Tests.Unit.Logging
{
    using System;
    using System.Threading;

    using log4net.Appender;
    using log4net.Core;

    /// <summary>
    /// Borrowed from https://github.com/cjbhaines/Log4Net.Async - will reference Nuget packages directly in v8
    /// </summary>
    public class DebugAppender : MemoryAppender
    {
        public TimeSpan AppendDelay { get; set; }
        public int LoggedEventCount { get { return this.m_eventsList.Count; } }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (this.AppendDelay > TimeSpan.Zero)
            {
                Thread.Sleep(this.AppendDelay);
            }
            base.Append(loggingEvent);
        }
    }
}