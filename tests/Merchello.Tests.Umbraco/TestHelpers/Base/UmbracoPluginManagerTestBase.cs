namespace Merchello.Tests.Umbraco.TestHelpers.Base
{
    using global::Umbraco.Core.Plugins;

    public abstract class UmbracoPluginManagerTestBase : UmbracoDataContextTestBase
    {

        protected PluginManager PluginManager { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            var serviceProvider = new global::Merchello.Core.ActivatorServiceProvider();

            // Umbraco's Plugin Manager
            this.PluginManager = new PluginManager(serviceProvider, this.CacheHelper.RuntimeCache, this.ProfileLogger, true);
        }
    }
}