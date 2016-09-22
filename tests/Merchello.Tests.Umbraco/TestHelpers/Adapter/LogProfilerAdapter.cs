namespace Merchello.Tests.Umbraco.TestHelpers.Adapter
{
    using System;

    using global::Umbraco.Core.Logging;

    using Merchello.Core;

    internal class LogProfilerAdapter : IProfiler
    {
        private readonly Core.Logging.IProfiler _profiler;


        public LogProfilerAdapter(Core.Logging.IProfiler profiler)
        {
            Ensure.ParameterNotNull(profiler, nameof(profiler));
            _profiler = profiler;
        }

        public string Render()
        {
            return _profiler.Render();
        }

        public IDisposable Step(string name)
        {
            return _profiler.Step(name);
        }

        public void Start()
        {
            _profiler.Start();
        }

        public void Stop(bool discardResults = false)
        {
            _profiler.Stop(discardResults);
        }
    }
}