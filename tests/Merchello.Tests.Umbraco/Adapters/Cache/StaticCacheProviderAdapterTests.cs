namespace Merchello.Tests.Umbraco.Adapters.Cache
{
    using Merchello.Core.Cache;
    using Merchello.Umbraco.Adapters;

    using NUnit.Framework;

    [TestFixture]
    public class StaticCacheProviderAdapterTests : CacheProviderAdapterTests
    {
        private ICacheProvider _staticCacheProvider;

        internal override ICacheProvider Provider
        {
            get
            {
                return this._staticCacheProvider;
            }
        }


        public override void Setup()
        {
            base.Setup();

            this._staticCacheProvider = new CacheProviderAdapter(this.CacheHelper.StaticCache);
        }
    }
}