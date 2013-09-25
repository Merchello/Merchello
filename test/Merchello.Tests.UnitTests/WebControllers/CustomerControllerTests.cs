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
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Hosting;

namespace Merchello.Tests.UnitTests.WebControllers
{
    [TestFixture]
    public class CustomerControllerTests : BaseRoutingTest
    {
        UmbracoContext tempUmbracoContext;

        protected override DatabaseBehavior DatabaseTestBehavior
        {
            get { return DatabaseBehavior.NoDatabasePerFixture; }
        }

        [SetUp]
        public void Setup()
        {
            tempUmbracoContext = GetRoutingContext("/test", 1234).UmbracoContext;
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

            MerchelloContext merchelloContext = new MerchelloContext(MockServiceContext.Object, null);

            CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);

            //// Act
            var result = ctrl.GetCustomer(customerKey);

            //// Assert
            Assert.AreEqual(result, customer);
        }

		/// <summary>
		/// Test to verify that the API throws an exception when param is not found
		/// </summary>
		[Test]
		public void GetCustomerThrowsWhenRepositoryReturnsNull()
		{
			// Arrange
			Guid customerKey = new Guid();

			var MockCustomerService = new Mock<ICustomerService>();
			MockCustomerService.Setup(cs => cs.GetByKey(customerKey)).Returns((Customer)null);

			MerchelloContext merchelloContext = GetMerchelloContext(MockCustomerService.Object);

			CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);

			var ex = Assert.Throws<HttpResponseException>(() => ctrl.GetCustomer(Guid.Empty));
		}

		/// <summary>
		/// Test to verify that the repository is updated on a PUT
		/// </summary>
		[Test]
		public void PutCustomerUpdatesRepository()
		{
			//// Arrange
			bool wasCalled = false;
			Guid customerKey = new Guid();

			Customer customer = new Customer(100.00m, 100.00m, DateTime.Now);
			customer.FirstName = "John";
			customer.LastName = "Jones";
			customer.Email = "john.jones@gmail.com";
			customer.MemberId = 1004;
			customer.Key = customerKey;
			customer.Id = 1001;

			var MockCustomerService = new Mock<ICustomerService>();
			MockCustomerService.Setup(cs => cs.Save(customer, It.IsAny<bool>())).Callback(() => wasCalled = true);

			MerchelloContext merchelloContext = GetMerchelloContext(MockCustomerService.Object);

			CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);
			ctrl.Request = new HttpRequestMessage();
			ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			//// Act
			HttpResponseMessage response = ctrl.PutCustomer(customer);

			//// Assert
			Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);

			Assert.True(wasCalled);
		}

		/// <summary>
		/// Test to verify that the proper error response is returned on an error
		/// </summary>
		//[Test]
		public void PutCustomerReturns500WhenRepositoryUpdateReturnsError()
		{
			//// Arrange
			Guid customerKey = new Guid();
			Customer customer = CreateFakeCustomer(customerKey);

			var MockCustomerService = new Mock<ICustomerService>();
			MockCustomerService.Setup(cs => cs.Save(customer, It.IsAny<bool>())).Throws<InvalidOperationException>();

			MerchelloContext merchelloContext = GetMerchelloContext(MockCustomerService.Object);

			CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);
			ctrl.Request = new HttpRequestMessage();
			ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			//// Act
			HttpResponseMessage response = ctrl.PutCustomer(customer);

			//// Assert
			Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Test to verify that the delete is called
		/// </summary>
		[Test]
		public void DeleteCustomerCallsRepositoryRemove()
		{
			//// Arrange
			Guid removedKey = Guid.Empty;

			Guid customerKey = new Guid();
			Customer customer = CreateFakeCustomer(customerKey);

			var MockCustomerService = new Mock<ICustomerService>();
			MockCustomerService.Setup(cs => cs.Delete(customer, It.IsAny<bool>())).Callback<ICustomer, bool>((p, b) => removedKey = p.Key);
			MockCustomerService.Setup(cs => cs.GetByKey(customerKey)).Returns(customer);

			MerchelloContext merchelloContext = GetMerchelloContext(MockCustomerService.Object);

			CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);
			ctrl.Request = new HttpRequestMessage();
			ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			//// Act
			HttpResponseMessage response = ctrl.Delete(customerKey);

			//// Assert
			Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

			Assert.AreEqual(customerKey, removedKey);
		}

		/// <summary>
		/// Test to verify that the customer is created
		/// </summary>
		[Test]
		public void NewCustomerReturnsCorrectProduct()
		{
			//// Arrange
			bool wasCalled = false;
			Guid customerKey = new Guid();
			Customer customer = CreateFakeCustomer(customerKey);

			var MockCustomerService = new Mock<ICustomerService>();
			MockCustomerService.Setup(cs => cs.CreateCustomer("John", "Jones", "john.jones@gmail.com", 1004)).Returns(customer).Callback(() => wasCalled = true);

			MerchelloContext merchelloContext = GetMerchelloContext(MockCustomerService.Object);

			CustomerApiController ctrl = new CustomerApiController(merchelloContext, tempUmbracoContext);

			//// Act
			Customer result = ctrl.NewCustomer("John", "Jones", "john.jones@gmail.com", 1004);

			//// Assert
			Assert.AreEqual(customer, result);
			Assert.True(wasCalled);
		}

		#region ServicesSetup

		/// <summary>
		/// Setup the Mocks and get a MerchelloContext
		/// </summary>
		/// <param name="mockProductService"></param>
		/// <returns>MerchelloContext</returns>
		private MerchelloContext GetMerchelloContext(ICustomerService mockCustomerService)
		{
			var MockServiceContext = new Mock<IServiceContext>();
			MockServiceContext.SetupGet(sc => sc.CustomerService).Returns(mockCustomerService);

			return new MerchelloContext(MockServiceContext.Object, null);
		}

		#endregion

		#region ProductSetup

		/// <summary>
		/// Create a fake product with fake data for testing
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private Customer CreateFakeCustomer(Guid key)
		{
			return new Customer(100.00m, 100.00m, DateTime.Now)
			{
				FirstName = "John",
				LastName = "Jones",
				Email = "john.jones@gmail.com",
				MemberId = 1004,
				Key = key,
				Id = 1001
			};	   			
		}

		#endregion
    }
}
