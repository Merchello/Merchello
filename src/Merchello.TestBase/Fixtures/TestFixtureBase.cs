namespace Merchello.TestBase.Fixtures
{
    using System;

    using Merchello.Core.Logging;

    using Moq;

    public abstract class TestFixtureBase : IDisposable
    {
        protected TestFixtureBase(ILogger logger)
        {
            this.Logger = logger;
        }

        public ILogger Logger { get; private set; }


        protected virtual bool EnableLogging => false;

        public virtual void Dispose()
        {
        }
    }
}