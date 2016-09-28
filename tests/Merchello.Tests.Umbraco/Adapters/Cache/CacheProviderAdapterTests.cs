namespace Merchello.Tests.Umbraco.Adapters.Cache
{
    using System;
    using System.Linq;
    using System.Web.UI;

    using global::Umbraco.Core.Cache;

    using NUnit.Framework;

    using umbraco;

    // Merchello does not have any cache providers - only an interface to Umbraco's
    // so we're using the modified versions of same tests writtent by Umbraco to ensure
    // the adapted version does the passes.
    [TestFixture]
    public abstract class CacheProviderAdapterTests
    {
        internal abstract global::Merchello.Core.Cache.ICacheProvider Provider { get; }

        internal CacheHelper CacheHelper { get; private set; }

        protected virtual int GetTotalItemCount
        {
            get
            {
                return this.Provider.GetCacheItemsByKeySearch("").Count();
            }
        }


        [OneTimeSetUp]
        public void Init()
        {

            // Umbraco's CacheHelper
            this.CacheHelper = new CacheHelper(
                new ObjectCacheRuntimeCacheProvider(),
                new StaticCacheProvider(),
                new NullCacheProvider(),
                new IsolatedRuntimeCache(type => new ObjectCacheRuntimeCacheProvider()));
        }

        [SetUp]
        public virtual void Setup()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
            this.Provider.ClearAllCache();
        }

        [Test]
        public void Throws_On_Reentry()
        {
            // don't run for StaticCacheProvider - not making sense
            if (this.GetType() == typeof(StaticCacheProviderAdapterTests))
                Assert.Ignore("Do not run for StaticCacheProviderAdapter.");

            Exception exception = null;
            var result = this.Provider.GetCacheItem("blah", () =>
            {
                try
                {
                    var result2 = this.Provider.GetCacheItem("blah");
                }
                catch (Exception e)
                {
                    exception = e;
                }
                return "value";
            });
            Assert.IsNotNull(exception);
            Assert.IsAssignableFrom<InvalidOperationException>(exception);
        }

        [Test]
        public void Does_Not_Cache_Exceptions()
        {
            var counter = 0;

            object result;
            try
            {
                result = this.Provider.GetCacheItem("Blah", () =>
                {
                    counter++;
                    throw new Exception("Do not cache this");
                });
            }
            catch (Exception) { }

            try
            {
                result = this.Provider.GetCacheItem("Blah", () =>
                {
                    counter++;
                    throw new Exception("Do not cache this");
                });
            }
            catch (Exception) { }

            Assert.Greater(counter, 1);

        }

        [Test]
        public void Ensures_Delegate_Result_Is_Cached_Once()
        {
            var counter = 0;

            object result;

            result = this.Provider.GetCacheItem("Blah", () =>
            {
                counter++;
                return "";
            });

            result = this.Provider.GetCacheItem("Blah", () =>
            {
                counter++;
                return "";
            });

            Assert.AreEqual(counter, 1);

        }

        [Test]
        public void Can_Get_By_Search()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            this.Provider.GetCacheItem("Tester2", () => cacheContent2);
            this.Provider.GetCacheItem("Tes3", () => cacheContent3);
            this.Provider.GetCacheItem("different4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            var result = this.Provider.GetCacheItemsByKeySearch("Tes");

            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void Can_Clear_By_Expression()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("TTes1t", () => cacheContent1);
            this.Provider.GetCacheItem("Tester2", () => cacheContent2);
            this.Provider.GetCacheItem("Tes3", () => cacheContent3);
            this.Provider.GetCacheItem("different4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            this.Provider.ClearCacheByKeyExpression("^\\w+es\\d.*");

            Assert.AreEqual(2, this.GetTotalItemCount);
        }

        [Test]
        public void Can_Clear_By_Search()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            this.Provider.GetCacheItem("Tester2", () => cacheContent2);
            this.Provider.GetCacheItem("Tes3", () => cacheContent3);
            this.Provider.GetCacheItem("different4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            this.Provider.ClearCacheByKeySearch("Test");

            Assert.AreEqual(2, this.GetTotalItemCount);
        }

        [Test]
        public void Can_Clear_By_Key()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            this.Provider.GetCacheItem("Test2", () => cacheContent2);
            this.Provider.GetCacheItem("Test3", () => cacheContent3);
            this.Provider.GetCacheItem("Test4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            this.Provider.ClearCacheItem("Test1");
            this.Provider.ClearCacheItem("Test2");

            Assert.AreEqual(2, this.GetTotalItemCount);
        }

        [Test]
        public void Can_Clear_All_Items()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            this.Provider.GetCacheItem("Test2", () => cacheContent2);
            this.Provider.GetCacheItem("Test3", () => cacheContent3);
            this.Provider.GetCacheItem("Test4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            this.Provider.ClearAllCache();

            Assert.AreEqual(0, this.GetTotalItemCount);
        }

        [Test]
        public void Can_Add_When_Not_Available()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            Assert.AreEqual(1, this.GetTotalItemCount);
        }

        [Test]
        public void Can_Get_When_Available()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var result = this.Provider.GetCacheItem("Test1", () => cacheContent1);
            var result2 = this.Provider.GetCacheItem("Test1", () => cacheContent1);
            Assert.AreEqual(1, this.GetTotalItemCount);
            Assert.AreEqual(result, result2);
        }

        [Test]
        public void Can_Remove_By_Type_Name()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            this.Provider.GetCacheItem("Test2", () => cacheContent2);
            this.Provider.GetCacheItem("Test3", () => cacheContent3);
            this.Provider.GetCacheItem("Test4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            //Provider.ClearCacheObjectTypes("umbraco.MacroCacheContent");
            this.Provider.ClearCacheObjectTypes(typeof(MacroCacheContent).ToString());

            Assert.AreEqual(1, this.GetTotalItemCount);
        }

        [Test]
        public void Can_Remove_By_Strong_Type()
        {
            var cacheContent1 = new MacroCacheContent(new LiteralControl(), "Test1");
            var cacheContent2 = new MacroCacheContent(new LiteralControl(), "Test2");
            var cacheContent3 = new MacroCacheContent(new LiteralControl(), "Test3");
            var cacheContent4 = new LiteralControl();
            this.Provider.GetCacheItem("Test1", () => cacheContent1);
            this.Provider.GetCacheItem("Test2", () => cacheContent2);
            this.Provider.GetCacheItem("Test3", () => cacheContent3);
            this.Provider.GetCacheItem("Test4", () => cacheContent4);

            Assert.AreEqual(4, this.GetTotalItemCount);

            this.Provider.ClearCacheObjectTypes<MacroCacheContent>();

            Assert.AreEqual(1, this.GetTotalItemCount);
        }
    }
}