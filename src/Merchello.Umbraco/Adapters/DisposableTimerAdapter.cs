namespace Merchello.Umbraco.Adapters
{
    using System.Diagnostics;

    using Merchello.Core;

    /// <summary>
    /// Represents an adapter for Umbraco's disposable time.
    /// </summary>
    internal class DisposableTimerAdapter : IDisposableTimer
    {
        /// <summary>
        /// The disposable timer.
        /// </summary>
        private readonly global::Umbraco.Core.DisposableTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableTimerAdapter"/> class.
        /// </summary>
        /// <param name="timer">
        /// Umbraco's disposable timer.
        /// </param>
        public DisposableTimerAdapter(global::Umbraco.Core.DisposableTimer timer)
        {
            Ensure.ParameterNotNull(timer, nameof(timer));

            _timer = timer;
        }

        /// <summary>
        /// Gets the stopwatch.
        /// </summary>
        public Stopwatch Stopwatch
        {
            get
            {
                return _timer.Stopwatch;
            }
        }

        /// <summary>
        /// Disposes the timer.
        /// </summary>
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}