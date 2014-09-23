namespace Merchello.Tests.Plugin.TestHelpers
{
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Tests.IntegrationTests.TestHelpers;
    using Merchello.Web;

    using Moq;

    using NUnit.Framework;

    using Umbraco.Web;

    using WebBootManager = Merchello.Web.WebBootManager;

    public class MerchelloBootstrapHelper
    {
       
        public ICustomerBase CurrentCustomer;
        public DbPreTestDataWorker DbPreTestDataWorker;

        private bool _isInitialized = false;
        
        public void Initialize()
        {
            if (_isInitialized) return;

            AutoMapperMappings.CreateMappings();


            var applicationMock = new Mock<UmbracoApplication>();

            // Sets Umbraco SqlSytax and ensure database is setup
            DbPreTestDataWorker = new DbPreTestDataWorker();
            DbPreTestDataWorker.ValidateDatabaseSetup();
            DbPreTestDataWorker.DeleteAllAnonymousCustomers();

            // Merchello CoreBootStrap
            var bootManager = new WebBootManager();
            bootManager.Initialize();


            if (MerchelloContext.Current == null) Assert.Ignore("MerchelloContext.Current is null");

            CurrentCustomer = DbPreTestDataWorker.MakeExistingAnonymousCustomer();

            _isInitialized = true;
        }

    }
}