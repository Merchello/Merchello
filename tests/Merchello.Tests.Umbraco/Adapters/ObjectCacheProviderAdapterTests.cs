namespace Merchello.Tests.Umbraco.Adapters
{
    using System.Linq;

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
                return _provider;
            }
        }

        internal override IRuntimeCacheProvider RuntimeProvider
        {
            get
            {
                return _provider;
            }
        }

        #endregion

        public override void Setup()
        {
            base.Setup();

            _provider = new RuntimeCacheProviderAdapter(CacheHelper.RuntimeCache);

        }
    }
}