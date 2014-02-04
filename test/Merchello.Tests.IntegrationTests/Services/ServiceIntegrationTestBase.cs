using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examine;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    public abstract class ServiceIntegrationTestBase
    {
        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            //ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
        }

        protected DbPreTestDataWorker PreTestDataWorker { get; private set; }

        protected ServiceIntegrationTestBase()
        {
            PreTestDataWorker = new DbPreTestDataWorker();
        }
    }
}
