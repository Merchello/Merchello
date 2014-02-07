using Merchello.Web;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    public abstract class DatabaseIntegrationTestBase
    {
        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            AutoMapperMappings.Initialize();
            //ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
        }

        protected DbPreTestDataWorker PreTestDataWorker { get; private set; }

        protected DatabaseIntegrationTestBase()
        {
            PreTestDataWorker = new DbPreTestDataWorker();
        }
    }
}
