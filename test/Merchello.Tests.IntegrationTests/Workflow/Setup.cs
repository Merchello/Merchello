using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Workflow
{
    [TestFixture]
    public class Setup : DatabaseIntegrationTestBase
    {
        [Test]
        public void init()
        {
            PreTestDataWorker.DeleteAllAnonymousCustomers();
            PreTestDataWorker.DeleteAllInvoices();
        }
    }
}
