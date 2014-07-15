using System;
using System.Runtime.InteropServices;
using Examine;
using Examine.Providers;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine.Providers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.IntegrationTests.Examine
{
    [TestFixture]
    public class CustomerProviderTests : DatabaseIntegrationTestBase
    {
        private CustomerIndexer _customerIndexer;
        private BaseSearchProvider _searcher;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var bootManager = new WebBootManager();
            bootManager.Initialize();

            _customerIndexer = (CustomerIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloCustomerIndexer"];
            _searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloCustomerSearcher"]

            CustomerService.Created += CustomerServiceCreated;
            CustomerService.Saved += CustomerServiceSaved;
            CustomerService.Deleted += CustomerServiceDeleted;


            //var invoiceProvider = (InvoiceIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"];
            //invoiceProvider.RebuildIndex();
        }




        private void CustomerServiceSaved(ICustomerService sender, SaveEventArgs<ICustomer> saveEventArgs)
        {
            foreach (var entity in saveEventArgs.SavedEntities)
            {
                _customerIndexer.AddCustomerToIndex(entity);
            }
        }

        private void CustomerServiceDeleted(ICustomerService sender, DeleteEventArgs<ICustomer> deleteEventArgs)
        {
            foreach(var entity in deleteEventArgs.DeletedEntities) _customerIndexer.DeleteCustomerFromIndex(entity);
        }

        private void CustomerServiceCreated(ICustomerService sender, Core.Events.NewEventArgs<ICustomer> newEventArgs)
        {           
           _customerIndexer.AddCustomerToIndex(newEventArgs.Entity);           
        }


     
    }
}