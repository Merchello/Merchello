namespace Merchello.Core.Logging
{
    using System;

    using Merchello.Core.Acquired;

    /// <summary>
    /// Represents a profiling logger.
    /// </summary>
    public interface IProfilingLogger
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the profiler.
        /// </summary>
        IProfiler Profiler { get; }

        /// <summary>
        /// Gets the disposable timer.
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
        IDisposableTimer DebugDuration(Type loggerType, string startMessage, string completeMessage);

        /// <summary>
        /// Gets the disposable timer.
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
        /// The minimum milliseconds threshold.
        /// </param>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        IDisposableTimer DebugDuration(Type loggerType, string startMessage, string completeMessage, int minimumMsThreshold);

        /// <summary>
        /// Gets the disposable timer.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <typeparam name="T">
        /// The type of the logger
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        IDisposableTimer DebugDuration<T>(string startMessage);

        /// <summary>
        /// Gets the disposable timer.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <typeparam name="T">
        /// The type of the logger
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        IDisposableTimer DebugDuration<T>(string startMessage, string completeMessage);

        /// <summary>
        /// Gets the disposable timer.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <param name="minimumMsThreshold">
        /// The minimum millisecond threshold.
        /// </param>
        /// <typeparam name="T">
        /// The type of the logger
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        IDisposableTimer DebugDuration<T>(string startMessage, string completeMessage, int minimumMsThreshold);

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
        IDisposableTimer TraceDuration(Type loggerType, string startMessage, string completeMessage);

        /// <summary>
        /// Gets the trace duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <typeparam name="T">
        /// The type of the logger.
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        IDisposableTimer TraceDuration<T>(string startMessage);

        /// <summary>
        /// Gets the trace duration.
        /// </summary>
        /// <param name="startMessage">
        /// The start message.
        /// </param>
        /// <param name="completeMessage">
        /// The complete message.
        /// </param>
        /// <typeparam name="T">
        /// The type of the logger
        /// </typeparam>
        /// <returns>
        /// The <see cref="DisposableTimer"/>.
        /// </returns>
        IDisposableTimer TraceDuration<T>(string startMessage, string completeMessage);
    }
}