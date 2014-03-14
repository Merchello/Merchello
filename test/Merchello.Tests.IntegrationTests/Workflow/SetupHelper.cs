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
    public class SetupHelper : MerchelloAllInTestBase
    {
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllInvoices();
        }

        [Test]
        public void doit()
        {
            
        }
    }
}
