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
    [TestFixture]
    public class InvoiceProviderTests : DatabaseIntegrationTestBase
    {
        private const int InvoiceCount = 2;
        private IAddress _address;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();


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
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            InvoiceService.Saved -= InvoiceServiceSaved;
        }

        private void InvoiceServiceSaved(IInvoiceService sender, SaveEventArgs<IInvoice> e)
        {
            
            var provider = (InvoiceIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"];
            foreach (var invoice in e.SavedEntities)
            {
                provider.AddInvoiceToIndex(invoice);
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
            var order = invoice3.PrepareOrder(MerchelloContext.Current);
            MerchelloContext.Current.Services.OrderService.Save(order);
            var key = invoice3.Key;

            //// Act
            var retrieved = MerchelloContext.Current.Services.InvoiceService.GetByKey(key);

            var provider = (InvoiceIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"];
            provider.AddInvoiceToIndex(retrieved);
            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloInvoiceSearcher"];

            var criteria = searcher.CreateSearchCriteria(Merchello.Examine.IndexTypes.Invoice);
            criteria.Field("invoiceKey", key.ToString());
            var results = searcher.Search(criteria);

            //// Assert
            Assert.AreEqual(1, results.Count());


        }

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
    }
}