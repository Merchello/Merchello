namespace Merchello.Tests.Umbraco.TestHelpers.Base
{
    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Services;

    using Merchello.Core.Boot;
    using Merchello.Tests.Umbraco.TestHelpers.Boot;

    public abstract class UmbracoApplicationContextBase : UmbracoPluginManagerTestBase
    {

        protected ApplicationContext ApplicationContext { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            // The application context (note: ALL of the Umbraco services are null)
            this.ApplicationContext = this.BuildApplicationContext(this.ProfileLogger);

            // Boot Merchello
            MerchelloBootstrapper.Init(new TestBoot(this.ApplicationContext, this.PluginManager));
        }

        /// Creates the Umbraco ApplicationContext
        private ApplicationContext BuildApplicationContext(ProfilingLogger profileLogger)
        {
            var serviceContext = new ServiceContext();

            return new ApplicationContext(this.DatabaseContext, serviceContext, this.CacheHelper, profileLogger);
        }
    }
}