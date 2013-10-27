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
    class InvoiceControllerTests : BaseRoutingTest
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
		/// Test to verify that the API gets the correct invoice by id
		/// </summary>
		[Test]
		public void GetInvoiceByKeyReturnsCorrectItemFromRepository()
		{
			// Arrange			
			int id = 1;
			Guid key = Guid.NewGuid();
			Invoice invoice = CreateFakeInvoice(1, key);

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.GetById(id)).Returns(invoice);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);

			//// Act
			var result = ctrl.GetInvoiceById(id);

			//// Assert
			Assert.AreEqual(invoice, result);
		}

		/// <summary>
		/// Test to verify that the API throws an exception when param is not found
		/// </summary>
		[Test]
		public void GetInvoiceThrowsWhenRepositoryReturnsNull()
		{
			// Arrange								  
			int id = 1;

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.GetById(id)).Returns((Invoice)null);

			var MockServiceContext = new Mock<IServiceContext>();
			MockServiceContext.SetupGet(sc => sc.InvoiceService).Returns(MockInvoiceService.Object);

			MerchelloContext merchelloContext = new MerchelloContext(MockServiceContext.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);

			var ex = Assert.Throws<HttpResponseException>(() => ctrl.GetInvoiceById(0));
		}

		/// <summary>
		/// Test to verify that the API gets the correct Invoices from the passed in Idss
		/// </summary>
		[Test]
		public void GetInvoicesByIdsReturnsCorrectItemsFromRepository()
		{
			// Arrange
			Guid key = Guid.NewGuid();

			int invoiceId1 = 1;		   
			Invoice invoice = CreateFakeInvoice(invoiceId1, key);

			int invoiceId2 = 2;
			Invoice invoice2 = CreateFakeInvoice(invoiceId2, key);

			int invoiceId3 = 3;
			Invoice invoice3 = CreateFakeInvoice(invoiceId3, key);

			List<Invoice> invoiceList = new List<Invoice>();
			invoiceList.Add(invoice);
			invoiceList.Add(invoice3);

			List<int> invoiceIds = new List<int>() { invoiceId1, invoiceId3 };

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.GetByIds(invoiceIds)).Returns(invoiceList);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);

			//// Act
			var result = ctrl.GetInvoicesByIds(invoiceIds);

			//// Assert
			Assert.AreEqual(invoiceList, result);
		}

		/// <summary>
		/// Test to verify that the API gets the correct Invoices from the passed in Idss
		/// </summary>
		[Test]
		public void GetInvoicesByCustomerReturnsCorrectItemsFromRepository()
		{
			//// Arrange			  
			Guid key = Guid.NewGuid();

			int invoiceId1 = 1;
			Invoice invoice = CreateFakeInvoice(invoiceId1, key);

			int invoiceId2 = 2;
			Invoice invoice2 = CreateFakeInvoice(invoiceId2, key);

			int invoiceId3 = 3;
			Invoice invoice3 = CreateFakeInvoice(invoiceId3, key);

			List<Invoice> invoiceList = new List<Invoice>();
			invoiceList.Add(invoice);
			invoiceList.Add(invoice2);
			invoiceList.Add(invoice3);

			List<int> invoiceIds = new List<int>() { invoiceId1, invoiceId3 };

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.GetInvoicesByCustomer(key)).Returns(invoiceList);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);

			//// Act
			var result = ctrl.GetInvoicesByCustomer(key);

			//// Assert
			Assert.AreEqual(invoiceList, result);
		}

		/// <summary>
		/// Test to verify that the API gets the correct Invoices from the passed in Idss
		/// </summary>
		[Test]
		public void GetInvoicesByinvoiceStatusIdReturnsCorrectItemsFromRepository()
		{
			//// Arrange			  
			Guid key = Guid.NewGuid();

			int invoiceId1 = 1;
			Invoice invoice = CreateFakeInvoice(invoiceId1, key);

			int invoiceId2 = 2;
			Invoice invoice2 = CreateFakeInvoice(invoiceId2, key);

			int invoiceId3 = 3;
			Invoice invoice3 = CreateFakeInvoice(invoiceId3, key);

			List<Invoice> invoiceList = new List<Invoice>();
			invoiceList.Add(invoice);
			invoiceList.Add(invoice2);
			invoiceList.Add(invoice3);

			int invoiceStatusId = 0;

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.GetInvoicesByInvoiceStatus(invoiceStatusId)).Returns(invoiceList);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);

			//// Act
			var result = ctrl.GetInvoicesByInvoiceStatus(invoiceStatusId);

			//// Assert
			Assert.AreEqual(invoiceList, result);
		}

		/// <summary>
		/// Test to verify that the repository is updated on a PUT
		/// </summary>
		[Test]
		public void PutInvoiceUpdatesRepository()
		{
			//// Arrange	
			Guid key = Guid.NewGuid();
			bool wasCalled = false;
			int id = 1;

			Invoice invoice = CreateFakeInvoice(id, key);

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.Save(invoice, It.IsAny<bool>())).Callback(() => wasCalled = true);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);
			ctrl.Request = new HttpRequestMessage();
			ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			//// Act
			HttpResponseMessage response = ctrl.PutInvoice(invoice);

			//// Assert
			Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

			Assert.True(wasCalled);
		}

		/// <summary>
		/// Test to verify that the proper error response is returned on an error
		/// </summary>
		//[Test]
		public void PutInvoiceReturns500WhenRepositoryUpdateReturnsError()
		{
			//// Arrange		
			Guid key = Guid.NewGuid();
			int id = 1;

			Invoice invoice = CreateFakeInvoice(id, key);

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.Save(invoice, It.IsAny<bool>())).Throws<InvalidOperationException>();

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);
			ctrl.Request = new HttpRequestMessage();
			ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			//// Act
			HttpResponseMessage response = ctrl.PutInvoice(invoice);

			//// Assert
			Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
		}

		/// <summary>
		/// Test to verify that the delete is called
		/// </summary>
		[Test]
		public void DeleteInvoiceCallsRepositoryRemove()
		{
			//// Arrange		
			Guid key = Guid.NewGuid();

			int removedId = 0;

			int id = 1;
			Invoice invoice = CreateFakeInvoice(id, key);

			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.Delete(invoice, It.IsAny<bool>())).Callback<IInvoice, bool>((p, b) => removedId = p.Id);
			MockInvoiceService.Setup(cs => cs.GetById(1)).Returns(invoice);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);
			ctrl.Request = new HttpRequestMessage();
			ctrl.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			//// Act
			HttpResponseMessage response = ctrl.Delete(id);

			//// Assert
			Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
		}

		/// <summary>
		/// Test to verify that the invoice is created
		/// </summary>
		[Test]
		public void NewInvoiceReturnsCorrectInvoice()
		{
			//// Arrange	   
			Guid key = Guid.NewGuid();
			bool wasCalled = false;
			int id = 1;
			Invoice invoice = CreateFakeInvoice(id, key);


			var customer = new AnonymousCustomer(100.00m, 100.00m, DateTime.Now);			 
				customer.FirstName = "John";
				customer.LastName = "Jones";
				customer.Email = "john.jones@gmail.com";
				customer.MemberId = 1004;
			
			var address = new CustomerAddress(customer, "Address")
			{
				Address1 = "123 Test St.",
				Address2 = "Apt 1",
				Locality = "USA",
				Region = "USA",
				PostalCode = "97333-0123",
				CountryCode = "US",
				Phone = "555-555-5555",
				Company = "Test Company"
			};
			var invoiceStatus = new InvoiceStatus()
			{
				Alias = "unpaid",
				Name = "Unpaid",
				Active = true,
				Reportable = true,
				SortOrder = 1
			};
			var MockInvoiceService = new Mock<IInvoiceService>();
			MockInvoiceService.Setup(cs => cs.CreateInvoice(customer, address, invoiceStatus, "Test Invoice 1")).Returns(invoice).Callback(() => wasCalled = true);

			MerchelloContext merchelloContext = GetMerchelloContext(MockInvoiceService.Object);

			InvoiceApiController ctrl = new InvoiceApiController(merchelloContext, tempUmbracoContext);

			//// Act
            Invoice result = null;
			//Invoice result = ctrl.NewInvoice(customer, address, invoiceStatus, "Test Invoice 1");

			//// Assert
			Assert.AreEqual(invoice, result);
			Assert.True(wasCalled);
		}

		#region ServicesSetup

		/// <summary>
		/// Setup the Mocks and get a MerchelloContext
		/// </summary>
		/// <param name="mockProductService"></param>
		/// <returns>MerchelloContext</returns>
		private MerchelloContext GetMerchelloContext(IInvoiceService mockInvoiceService)
		{
			var MockServiceContext = new Mock<IServiceContext>();
			MockServiceContext.SetupGet(sc => sc.InvoiceService).Returns(mockInvoiceService);

			return new MerchelloContext(MockServiceContext.Object);
		}

		#endregion

		#region ProductSetup

		/// <summary>
		/// Create a fake product with fake data for testing
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private Invoice CreateFakeInvoice(int id, Guid key)
		{
			var customer = new AnonymousCustomer(100.00m, 100.00m, DateTime.Now)
			{
				FirstName = "John",
				LastName = "Jones",
				Email = "john.jones@gmail.com",
				MemberId = 1004,
				Key = key,
				Id = 1001
			};
			var address = new CustomerAddress(customer, "Address")
			{
				Address1 = "123 Test St.",
				Address2 = "Apt 1",
				Locality = "USA",
				Region = "USA",
				PostalCode = "97333-0123",
				CountryCode = "US",
				Phone = "555-555-5555",
				Company = "Test Company"
			};

			var invoiceStatus = new InvoiceStatus()
			{
				Alias = "unpaid", 
				Name = "Unpaid",
				Active = true,
				Reportable = true,
				SortOrder = 1
			};
			return new Invoice(customer, address, invoiceStatus, 100.00m)
			{
				Id = id
			};
		}

		#endregion
    }
}
