﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using Examine;
using Examine.Providers;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine;
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
        private ICustomerService _customerService;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var bootManager = new WebBootManager();
            bootManager.Initialize();

            _customerIndexer = (CustomerIndexer)ExamineManager.Instance.IndexProviderCollection["MerchelloCustomerIndexer"];
            _searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloCustomerSearcher"];

            _customerService = PreTestDataWorker.CustomerService;

            CustomerService.Created += CustomerServiceCreated;
            CustomerService.Saved += CustomerServiceSaved;
            CustomerService.Deleted += CustomerServiceDeleted;

        }

        [SetUp]
        public void Initialize()
        {
            PreTestDataWorker.DeleteAllCustomers();
        }

        /// <summary>
        /// Test shows that a customer can be saved and indexed
        /// </summary>
        [Test]
        public void Can_Save_A_Customer_And_Find_It_In_The_Index()
        {
            //// Arrange
            var customer = _customerService.CreateCustomerWithKey(
                "rusty",
                "firstName",
                "lastName",
                "test@test.com");

            //// Act
            var criteria = _searcher.CreateSearchCriteria(IndexTypes.Customer);
            criteria.Field("loginName", "rusty");
            var results = _searcher.Search(criteria);

            //// Assert
            Assert.IsTrue(results.Any());
        }


        public void Can_Retrieve_A_CustomerDisplay_From_The_Index()
        {
            
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            CustomerService.Created -= CustomerServiceCreated;
            CustomerService.Saved -= CustomerServiceSaved;
            CustomerService.Deleted -= CustomerServiceDeleted;
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