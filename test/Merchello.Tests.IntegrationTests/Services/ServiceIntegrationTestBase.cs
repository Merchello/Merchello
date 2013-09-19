using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Tests.IntegrationTests.TestHelpers;

namespace Merchello.Tests.IntegrationTests.Services
{
    public abstract class ServiceIntegrationTestBase
    {
        protected DbPreTestDataWorker PreTestDataWorker { get; private set; }

        protected ServiceIntegrationTestBase()
        {
            PreTestDataWorker = new DbPreTestDataWorker();
        }
    }
}
