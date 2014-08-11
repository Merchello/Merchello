using System;
using System.Diagnostics;
using System.Linq;
using Examine;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine.Providers;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.IntegrationTests.Examine
{
    using Merchello.Web.Search;

    [TestFixture]
    public class InvoiceAndOrderProviderTests : DatabaseIntegrationTestBase
    {
        private const int InvoiceCount = 2;
        private IAddress _address;
        
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            PreTestDataWorker.DeleteAllCustomers();

            var bootManager = new WebBootManager();
            bootManager.Initialize();

            _address = new Address()
                {
                    Name = "Test",
                    Address1 = "111 Somewhere Outwhere",
                    Locality = "Out There",
                    PostalCode = "98225",
                    Region = "WA",
                    CountryCode = "US",
                    Email = "test@test.com"
                };

            InvoiceService.Saved += InvoiceServiceSaved;

            OrderService.Saved += OrderServiceSaved;

            //var invoiceProvider = (InvoiceIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"];
            //invoiceProvider.RebuildIndex();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            InvoiceService.Saved -= InvoiceServiceSaved;
            OrderService.Saved -= OrderServiceSaved;
        }

        private void InvoiceServiceSaved(IInvoiceService sender, SaveEventArgs<IInvoice> e)
        {
            
            var provider = (InvoiceIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"];
            foreach (var invoice in e.SavedEntities)
            {
                provider.AddInvoiceToIndex(invoice);
            }
            
        }

        private void OrderServiceSaved(IOrderService sender, SaveEventArgs<IOrder> e)
        {
            var provider = (OrderIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloOrderIndexer"];
            foreach (var order in e.SavedEntities)
            {
                provider.AddOrderToIndex(order);
            }
        }

        /// <summary>
        /// Test to verify that the invoice index can be rebuilt
        /// </summary>
        [Test]
        public void Can_Rebuild_Invoice_Index()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllInvoices();
            var invoice1 = MockInvoiceDataMaker.InvoiceForInserting(_address, 100);
            var invoice2 = MockInvoiceDataMaker.InvoiceForInserting(_address, 200);

            PreTestDataWorker.InvoiceService.Save(invoice1);
            PreTestDataWorker.InvoiceService.Save(invoice2);

            //// Act
            var timer = new Stopwatch();
            timer.Start();
            ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"].RebuildIndex();
            timer.Stop();
            Console.Write("Time to index: " + timer.Elapsed.ToString());

            //// Assert
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloInvoiceSearcher"];

            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.Invoice);
            criteria.Field("allDocs", "1");
            var results = searcher.Search(criteria);

            Assert.AreEqual(2, results.Count());
        }

        /// <summary>
        /// Test to verify that the order index can be rebuilt
        /// </summary>
        [Test]
        public void Can_Rebuild_Order_Index()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllOrders();
            var invoice1 = MockInvoiceDataMaker.InvoiceForInserting(_address, 100);
            var invoice2 = MockInvoiceDataMaker.InvoiceForInserting(_address, 200);

            PreTestDataWorker.InvoiceService.Save(invoice1);
            PreTestDataWorker.InvoiceService.Save(invoice2);

            var order1 = invoice1.PrepareOrder();
            var order2 = invoice2.PrepareOrder();

            PreTestDataWorker.OrderService.Save(order1);
            PreTestDataWorker.OrderService.Save(order2);

            //// Act
            var timer = new Stopwatch();
            timer.Start();
            ExamineManager.Instance.IndexProviderCollection["MerchelloOrderIndexer"].RebuildIndex();
            timer.Stop();
            Console.Write("Time to index: " + timer.Elapsed.ToString());

            //// Assert
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloOrderSearcher"];

            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.Order);
            criteria.Field("allDocs", "1");
            var results = searcher.Search(criteria);

            Assert.AreEqual(2, results.Count());
        }

        /// <summary>
        /// Test verifies that an invoice can be added to the index
        /// </summary>
        [Test]
        public void Can_Add_An_Invoice_ToIndex()
        {
            //// Arrange
            var invoice3 = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 2, 100));
            PreTestDataWorker.InvoiceService.Save(invoice3);
            var order = invoice3.PrepareOrder(Core.MerchelloContext.Current);
            Core.MerchelloContext.Current.Services.OrderService.Save(order);
            var key = invoice3.Key;

            //// Act
            Core.MerchelloContext.Current.Services.InvoiceService.GetByKey(key);         
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloInvoiceSearcher"];

            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.Invoice);
            criteria.Field("invoiceKey", key.ToString());
            var results = searcher.Search(criteria);

            //// Assert
            Assert.AreEqual(1, results.Count());
        }

        /// <summary>
        /// Test verifies that a collection of invoices can be retrieved from the index by customer
        /// </summary>
        [Test]
        public void Can_Retrieve_Invoices_By_Customer_From_The_Index()
        {
            //// Arrange

            var customer = PreTestDataWorker.CustomerService.CreateCustomerWithKey(
                "rusty",
                "firstName",
                "lastName",
                "test@test.com");

            var invoice1 = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice1.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            invoice1.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 2, 100));
            ((Invoice)invoice1).CustomerKey = customer.Key;

            var invoice2 = MockInvoiceDataMaker.InvoiceForInserting(_address, 100);
            invoice2.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            ((Invoice)invoice2).CustomerKey = customer.Key;

            var invoice3 = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 3, 100));
            ((Invoice)invoice3).CustomerKey = customer.Key;

            PreTestDataWorker.InvoiceService.Save(invoice1);
            PreTestDataWorker.InvoiceService.Save(invoice2);
            PreTestDataWorker.InvoiceService.Save(invoice3);

            //// Act
            var merchello = new MerchelloHelper();

            var invoices = merchello.InvoicesByCustomer(customer.Key);

            //// Assert
            Assert.NotNull(invoices, "invoices was null");
            Assert.IsTrue(invoices.Any());
            Assert.AreEqual(3, invoices.Count());
        }

        /// <summary>
        /// Test verifies that an event updates the index
        /// </summary>
        [Test]
        public void Can_Updates_Index_With_InvoiceService_SaveEvent()
        {
            //// Arrange
            
            //// Act
            var invoice3 = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 2, 100));
            PreTestDataWorker.InvoiceService.Save(invoice3);

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloInvoiceSearcher"];

            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.Invoice);
            criteria.Field("invoiceKey", invoice3.Key.ToString());
            var results = searcher.Search(criteria);

            //// Assert
            Assert.AreEqual(1, results.Count());

        }

        /// <summary>
        /// Test verifies that an order can be added to an index
        /// </summary>
        [Test]
        public void Can_Add_An_Order_ToIndex_SaveEvent()
        {
            //// Arrange
            var invoice3 = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            invoice3.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 2, 100));
            PreTestDataWorker.InvoiceService.Save(invoice3);
            var order = invoice3.PrepareOrder(Core.MerchelloContext.Current);
            
            //// Act
            Core.MerchelloContext.Current.Services.OrderService.Save(order);
            var key = order.Key;

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloOrderSearcher"];

            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.Order);
            criteria.Field("orderKey", key.ToString());
            var results = searcher.Search(criteria);

            //// Assert
            Assert.AreEqual(1, results.Count());
        }

        /// <summary>
        /// Test verifies that an InvoiceDisplay can be created from a document in the index
        /// </summary>
        [Test]
        public void Can_Create_InvoiceDiplay_From_Index_Document()
        {
            //// Arrange
            var invoice = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            invoice.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 2, 100));
            PreTestDataWorker.InvoiceService.Save(invoice);

            //// Act
            var invoiceDisplay = InvoiceQuery.GetByKey(invoice.Key);

            //// Assert
            Assert.NotNull(invoiceDisplay);
            Assert.AreEqual(Constants.DefaultKeys.InvoiceStatus.Unpaid, invoiceDisplay.InvoiceStatus.Key);
            Assert.AreEqual(invoice.Items.Count, invoiceDisplay.Items.Count());
        }

        /// <summary>
        /// Test verifies that an OrderDisplay can be created from a document in the index
        /// </summary>
        [Test]
        public void Can_Create_OrderDisplay_From_Index_Document()
        {
            //// Arrange
            var invoice = MockInvoiceDataMaker.InvoiceForInserting(_address, 300);
            invoice.Items.Add(new InvoiceLineItem(LineItemType.Product, "test", "test", 1, 100));
            invoice.Items.Add(new InvoiceLineItem(LineItemType.Product, "test2", "test2", 2, 100));
            PreTestDataWorker.InvoiceService.Save(invoice);

            var order = invoice.PrepareOrder();
            Core.MerchelloContext.Current.Services.OrderService.Save(order);

            //// Act
            var orderDisplay = OrderQuery.GetByKey(order.Key);

            //// Assert
            Assert.NotNull(orderDisplay);
            Assert.AreEqual(Constants.DefaultKeys.OrderStatus.NotFulfilled, orderDisplay.OrderStatus.Key);
            Assert.AreEqual(order.Items.Count, orderDisplay.Items.Count());
        }
    }
}