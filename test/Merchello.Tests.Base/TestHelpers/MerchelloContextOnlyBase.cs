namespace Merchello.Tests.Base.TestHelpers
{
    using Merchello.Core;
    using Merchello.Web;

    using NUnit.Framework;

    public abstract class MerchelloContextOnlyBase
    {
        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            // Sets Umbraco SqlSytax and ensure database is setup
            var dbPreTestDataWorker = new DbPreTestDataWorker();

            // Merchello CoreBootStrap
            var bootManager = new WebBootManager(dbPreTestDataWorker.TestLogger);
            bootManager.Initialize();


            if (MerchelloContext.Current == null) Assert.Ignore("MerchelloContext.Current is null");
        }
    }
}