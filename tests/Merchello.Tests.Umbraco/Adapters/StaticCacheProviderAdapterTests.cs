namespace Merchello.Tests.Umbraco.Adapters
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
                return _staticCacheProvider;
            }
        }


        public override void Setup()
        {
            base.Setup();

            _staticCacheProvider = new CacheProviderAdapter(CacheHelper.StaticCache);
        }
    }
}