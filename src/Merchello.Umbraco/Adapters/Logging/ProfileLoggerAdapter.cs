namespace Merchello.Umbraco.Adapters.Logging
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Logging;

    /// <inheritdoc/>
    internal class ProfilerAdapter : IProfiler
    {
        /// <summary>
        /// Umbraco's profiler.
        /// </summary>
        private readonly global::Umbraco.Core.Logging.IProfiler _profiler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilerAdapter"/> class.
        /// </summary>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        public ProfilerAdapter(global::Umbraco.Core.Logging.IProfiler profiler)
        {
            Ensure.ParameterNotNull(profiler, nameof(profiler));

            this._profiler = profiler;
        }

        /// <inheritdoc/>
        public string Render()
        {
            return this._profiler.Render();
        }

        /// <inheritdoc/>
        public IDisposable Step(string name)
        {
            return this._profiler.Step(name);
        }

        /// <inheritdoc/>
        public void Start()
        {
            this._profiler.Start();
        }

        /// <inheritdoc/>
        public void Stop(bool discardResults = false)
        {
            this._profiler.Stop(discardResults);
        }
    }
}