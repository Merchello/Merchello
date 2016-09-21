namespace Merchello.Tests.Umbraco.Adapters
{
    using Merchello.Core.Cache;
    using Merchello.Umbraco.Adapters;

    using NUnit.Framework;

    [TestFixture]
    public class CacheProviderAdapterTests
    {
        private global::Umbraco.Core.Cache.ICacheProvider CacheProvider;

        [SetUp]
        public void Init()
        {
            this.CacheProvider = new global::Umbraco.Core.Cache.StaticCacheProvider();
        }

        [Test]
        public void CacheProviderAdapter()
        {
            var adapted = new CacheProviderAdapter(CacheProvider);

            var provider = adapted as ICacheProvider;

            Assert.NotNull(provider);
        }
    }
}