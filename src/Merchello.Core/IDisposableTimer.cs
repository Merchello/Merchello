namespace Merchello.Core
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a disposable timer.
    /// </summary>
    /// <remarks>
    /// Used for profiling.
    /// </remarks>
    public interface IDisposableTimer
    {
        /// <summary>
        /// Gets the <see cref="Stopwatch"/>.
        /// </summary>
        Stopwatch Stopwatch { get; }
    }
}