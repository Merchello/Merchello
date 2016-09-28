namespace Merchello.Umbraco.Adapters.Logging
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Logging;

    /// <summary>
    /// Represents an adapter for Umbraco's profiling logger.
    /// </summary>
    internal sealed class ProfilingLoggerAdapter : IProfilingLogger
    {
        /// <summary>
        /// Umbraco's profiling logger.
        /// </summary>
        private readonly global::Umbraco.Core.Logging.ProfilingLogger _profileLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilingLoggerAdapter"/> class.
        /// </summary>
        /// <param name="profileLogger">
        /// Umbraco's profiling logger.
        /// </param>
        public ProfilingLoggerAdapter(global::Umbraco.Core.Logging.ProfilingLogger profileLogger)
        {
            Ensure.ParameterNotNull(profileLogger, nameof(profileLogger));

            this._profileLogger = profileLogger;

            this.Logger = new LoggerAdapter(this._profileLogger.Logger);
            this.Profiler = new ProfilerAdapter(this._profileLogger.Profiler);
        }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        /// <inheritdoc/>
        public IProfiler Profiler { get; }

        /// <inheritdoc/>
        public IDisposableTimer DebugDuration(Type loggerType, string startMessage, string completeMessage)
        {
            return new DisposableTimerAdapter(this._profileLogger.DebugDuration(loggerType, startMessage, completeMessage));
        }

        /// <inheritdoc/>
        public IDisposableTimer DebugDuration(Type loggerType, string startMessage, string completeMessage, int minimumMsThreshold)
        {
            return new DisposableTimerAdapter(this._profileLogger.DebugDuration(loggerType, startMessage, completeMessage, minimumMsThreshold));
        }

        /// <inheritdoc/>
        public IDisposableTimer DebugDuration<T>(string startMessage)
        {
            return new DisposableTimerAdapter(this._profileLogger.DebugDuration<T>(startMessage));
        }

        /// <inheritdoc/>
        public IDisposableTimer DebugDuration<T>(string startMessage, string completeMessage)
        {
            return new DisposableTimerAdapter(this._profileLogger.DebugDuration<T>(startMessage, completeMessage));
        }

        /// <inheritdoc/>
        public IDisposableTimer DebugDuration<T>(string startMessage, string completeMessage, int minimumMsThreshold)
        {
            return new DisposableTimerAdapter(this._profileLogger.DebugDuration<T>(startMessage, completeMessage, minimumMsThreshold));
        }

        /// <inheritdoc/>
        public IDisposableTimer TraceDuration(Type loggerType, string startMessage, string completeMessage)
        {
            return new DisposableTimerAdapter(this._profileLogger.TraceDuration(loggerType, startMessage, completeMessage));
        }

        /// <inheritdoc/>
        public IDisposableTimer TraceDuration<T>(string startMessage)
        {
            return new DisposableTimerAdapter(this._profileLogger.TraceDuration<T>(startMessage));
        }

        /// <inheritdoc/>
        public IDisposableTimer TraceDuration<T>(string startMessage, string completeMessage)
        {
            return new DisposableTimerAdapter(this._profileLogger.TraceDuration<T>(startMessage, completeMessage));
        }
    }
}