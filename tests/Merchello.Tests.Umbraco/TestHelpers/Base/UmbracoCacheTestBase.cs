namespace Merchello.Tests.Umbraco.TestHelpers.Base
{
    using global::Umbraco.Core.Cache;

    public abstract class UmbracoCacheTestBase : UmbracoLoggerTestBase
    {
        protected CacheHelper CacheHelper { get; private set; }

        protected virtual bool EnableCache
        {
            get
            {
                return false;
            }
        }


        public override void Initialize()
        {
            base.Initialize();

            // Create an Umbraco CacheHelper
            this.CacheHelper = this.EnableCache
                                   ? new CacheHelper(
                                         new ObjectCacheRuntimeCacheProvider(),
                                         new StaticCacheProvider(),
                                         new NullCacheProvider(),
                                         new IsolatedRuntimeCache(type => new ObjectCacheRuntimeCacheProvider()))
                                   : CacheHelper.CreateDisabledCacheHelper();
        }

    }
}