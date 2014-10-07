using NUnit.Framework;

namespace Merchello.Tests.Chase.Integration.TestHelpers
{
    public abstract class ProviderTestsBase : ChaseTestBase
    {                                                      
        
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            base.TestFixtureSetup();
            // Sets Umbraco SqlSytax and ensure database is setup

        }
    }
}