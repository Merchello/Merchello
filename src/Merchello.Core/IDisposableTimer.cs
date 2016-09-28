namespace Merchello.Core
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents a disposable timer.
    /// </summary>
    /// <remarks>
    /// Used for profiling.
    /// </remarks>
    public interface IDisposableTimer : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="Stopwatch"/>.
        /// </summary>
        Stopwatch Stopwatch { get; }
    }
}