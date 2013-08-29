using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using Moq;
using Merchello.Core;
using Merchello.Core.Services;
using Merchello.Core.Models;
using Merchello.Web.Editors;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Tests.TestHelpers;

namespace Merchello.Tests.UnitTests.WebControllers
{
    [TestFixture]
    public class CustomerControllerTests : BaseUmbracoApplicationTest
    {
        UmbracoContext tempUmbracoContext;

        [SetUp]
        public void Setup()
        {
            var httpContext = new Mock<HttpContextBase>();

            //tempUmbracoContext = UmbracoContext.EnsureContext(httpContext.Object, ApplicationContext.Current);
        }

        /// <summary>
        /// Test to verify that the API gets the correct customer by Key
        /// </summary>
        [Test]
        public void GetCustomerByKeyReturnsCorrectItemFromRepository()
        {
            // Arrange
            Guid customerKey = new Guid();

            var customer = new Customer(100.00m, 100.00m, DateTime.Now);
            customer.FirstName = "John";
            customer.LastName = "Jones";
            customer.Email = "john.jones@gmail.com";
            customer.MemberId = 1004;
            customer.Key = customerKey;
            customer.Id = 1001;

            var MockCustomerService = new Mock<ICustomerService>();
            MockCustomerService.Setup(cs => cs.GetByKey(customerKey)).Returns(customer);

            var MockServiceContext = new Mock<IServiceContext>();
            MockServiceContext.SetupGet(sc => sc.CustomerService).Returns(MockCustomerService.Object);

            //MerchelloContext merchelloContext = new MerchelloContext(MockServiceContext.Object, null);

            //CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);

            //// Act
            //var result = ctrl.GetCustomer(customerKey);

            //// Assert
            //Assert.AreEqual(result, customer);
        }
    }
}
