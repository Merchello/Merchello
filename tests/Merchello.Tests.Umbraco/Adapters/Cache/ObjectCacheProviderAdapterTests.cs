namespace Merchello.Tests.Umbraco.Adapters.Cache
{
    using Merchello.Core.Cache;
    using Merchello.Umbraco.Adapters;

    using NUnit.Framework;

    [TestFixture]
    public class ObjectCacheProviderAdapterTests : RuntimeCacheProviderAdapterTests
    {
        private IRuntimeCacheProvider _provider;

        #region Properties

        internal override ICacheProvider Provider
        {
            get
            {
                return this._provider;
            }
        }

        internal override IRuntimeCacheProvider RuntimeProvider
        {
            get
            {
                return this._provider;
            }
        }

        #endregion

        public override void Setup()
        {
            base.Setup();

            this._provider = new RuntimeCacheProviderAdapter(this.CacheHelper.RuntimeCache);

        }
    }
}