using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Customer
{
    [TestFixture]
    [Category("Service Integration")]
    public class CustomerServiceTests : ServiceIntegrationTestBase
    {
        private ICustomerService _customerService;
        

        [SetUp]
        public void Initialize()
        {
            _customerService = PreTestDataWorker.CustomerService;
        }


    }
}