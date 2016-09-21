namespace Merchello.Tests.Umbraco.Adapters
{
    using System;
    using System.Threading;

    using Merchello.Core.Cache;
    using Merchello.Core.Acquired.Cache;

    using NUnit.Framework;

    public abstract class RuntimeCacheProviderAdapterTests : CacheProviderAdapterTests
    {
        internal abstract IRuntimeCacheProvider RuntimeProvider { get; }

        [Test]
        public void Can_Add_And_Expire_Struct_Strongly_Typed_With_Null()
        {
            var now = DateTime.Now;
            RuntimeProvider.InsertCacheItem("DateTimeTest", () => now, new TimeSpan(0, 0, 0, 0, 200));
            Assert.AreEqual(now, Provider.GetCacheItem<DateTime>("DateTimeTest"));
            Assert.AreEqual(now, Provider.GetCacheItem<DateTime?>("DateTimeTest"));

            Thread.Sleep(300); //sleep longer than the cache expiration

            Assert.AreEqual(default(DateTime), Provider.GetCacheItem<DateTime>("DateTimeTest"));
            Assert.AreEqual(null, Provider.GetCacheItem<DateTime?>("DateTimeTest"));
        }

    }
}