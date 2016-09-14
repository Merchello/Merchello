namespace Merchello.Solo.Logging
{
    using System;
    using Merchello.Core.Logging;

    /// <summary>
    /// Used to create DisposableTimer instances for debugging or tracing durations
    /// </summary>
    /// <remarks>
    /// Modified port of Umbraco's profile logger. This has been made internal to reduce ambiguity in the IDE
    /// </remarks>
    internal sealed class ProfilingLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilingLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        public ProfilingLogger(ILogger logger, IProfiler profiler)
        {
            this.Logger = logger;
            this.Profiler = profiler;
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (profiler == null) throw new ArgumentNullException(nameof(profiler));
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the profiler.
        /// </summary>
        public IProfiler Profiler { get; }

        /// <summary>
        /// Get a trace duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer TraceDuration<T>(string startMessage, string completeMessage)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Info, this.Profiler, typeof(T), startMessage, completeMessage);
        }

        /// <summary>
        /// Gets the trace duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer TraceDuration<T>(string startMessage)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Info, this.Profiler, typeof(T), startMessage, "Complete");
        }

        /// <summary>
        /// Gets the trace duration.
        /// </summary>
        /// <param name="loggerType">
        /// The logger type.
        /// </param>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer TraceDuration(Type loggerType, string startMessage, string completeMessage)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Info, this.Profiler, loggerType, startMessage, completeMessage);
        }

        /// <summary>
        /// Gets the debug duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer DebugDuration<T>(string startMessage, string completeMessage)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Debug, this.Profiler, typeof(T), startMessage, completeMessage);
        }

        /// <summary>
        /// Gets the debug duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <param name="minimumMsThreshold">
        /// The minimum milliseconds threshold.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer DebugDuration<T>(string startMessage, string completeMessage, int minimumMsThreshold)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Debug, this.Profiler, typeof(T), startMessage, completeMessage, minimumMsThreshold);
        }

        /// <summary>
        /// Gets the debug duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <typeparam name="T">
        /// The calling type
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer DebugDuration<T>(string startMessage)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Debug, this.Profiler, typeof(T), startMessage, "Complete");
        }

        /// <summary>
        /// Gets the debug duration.
        /// </summary>
        /// <param name="loggerType">
        /// The logger type.
        /// </param>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer DebugDuration(Type loggerType, string startMessage, string completeMessage)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Debug, this.Profiler, loggerType, startMessage, completeMessage);
        }

        /// <summary>
        /// Gets the debug duration.
        /// </summary>
        /// <param name="loggerType">
        /// The logger type.
        /// </param>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <param name="minimumMsThreshold">
        /// The minimum ms threshold.
        /// </param>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        public DisposableTimer DebugDuration(Type loggerType, string startMessage, string completeMessage, int minimumMsThreshold)
        {
            return new DisposableTimer(this.Logger, DisposableTimer.LogType.Debug, this.Profiler, loggerType, startMessage, completeMessage, minimumMsThreshold);
        }
    }
}