using Merchello.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.Avalara.Integration.TestBase
{
    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;
    using Merchello.Plugin.Taxation.Avalara.Services;
    using Merchello.Tests.Avalara.Integration.TestBase.Mocks;
    using Merchello.Tests.IntegrationTests.TestHelpers;

    using Moq;

    using NUnit.Framework;

    using Umbraco.Core.Cache;

    public class AvaTaxTestBase : MerchelloAllInTestBase
    {
        protected IGatewayProviderSettings GatewayProviderSettings;
        protected IGatewayProviderService GatewayProviderService;
        protected DbPreTestDataWorker DataWorker;
        protected IAvaTaxService AvaTaxService;

        protected IInvoice Invoice;


        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            AutoMapper.Mapper.CreateMap<IValidatableAddress, TaxAddress>(); 

            AvaTaxService = new AvaTaxService(TestHelper.GetAvaTaxProviderSettings());

            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            DataWorker = new DbPreTestDataWorker(new ServiceContext(new PetaPocoUnitOfWorkProvider()));

            MakeInvoice();
        }

        private void MakeInvoice()
        {

            var origin = new Address()
            {
                Organization = "Mindfly Web Design Studios",
                Address1 = "114 W. Magnolia St. Suite 300",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
                Email = "someone@mindfly.com",
                Phone = "555-555-5555"
            };

            //var billToShipTo = new Address()
            //    {
            //        Name = "The President of the United States",
            //        Address1 = "1600 Pennsylvania Ave NW",
            //        Locality = "Washington",
            //        Region = "DC",
            //        PostalCode = "20500",
            //        CountryCode = "US",                 
            //    };

            var billToShipTo = new Address()
            {
                Name = "Old Office",
                Address1 = "211 W Holly St H22",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
            };

            var invoiceService = new InvoiceService();

            Invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            Invoice.SetBillingAddress(billToShipTo);

            Invoice.Total = 120M;
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, true.ToString());

            // make up some line items
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 10, 1, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Product, "Item 2", "I2", 2, 40, extendedData);

            Invoice.Items.Add(l1);
            Invoice.Items.Add(l2);
            
            var shipment = new ShipmentMock(origin, billToShipTo, Invoice.Items);

            var shipmethod = new ShipMethodMock();

            var quote = new ShipmentRateQuote(shipment, shipmethod) { Rate = 16.22M };
            Invoice.Items.Add(quote.AsLineItemOf<InvoiceLineItem>());
        }
    }
}