namespace Merchello.Tests.Umbraco.Adapters
{
    using Merchello.Core.Cache;
    using Merchello.Tests.Umbraco.TestHelpers;
    using Merchello.Umbraco.Adapters;

    using NUnit.Framework;

    [TestFixture]
    public class CacheHelperAdapterTests : UmbracoInstanceBase
    {
        protected override bool EnableCache { get { return false; } }

        [Test]
        public void CacheHelperAdapter()
        {
            var adapted = new CacheHelperAdapter(CacheHelper);

            Assert.NotNull(adapted.RuntimeCache);
            Assert.NotNull(adapted.StaticCache);
            Assert.NotNull(adapted.RequestCache);
            Assert.NotNull(adapted.IsolatedRuntimeCache);
        }
    }
}