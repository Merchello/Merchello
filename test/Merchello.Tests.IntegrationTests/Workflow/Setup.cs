using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Workflow
{
    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Web;
    using Merchello.Web.Workflow;

    [TestFixture]
    public class Setup : MerchelloAllInTestBase
    {
        private const int ProductCount = 55;

        private const int InvoiceCount = 5562;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

        }

        public void init()
        {
            DbPreTestDataWorker.DeleteAllAnonymousCustomers();
            DbPreTestDataWorker.DeleteAllInvoices();
        }

        public void ReportTestsSetup()
        {
            DbPreTestDataWorker.DeleteAllProducts();
            DbPreTestDataWorker.DeleteAllInvoices();
            DbPreTestDataWorker.DeleteAllShipCountries();

            this.MakeProducts();

            var defaultWarehouse = DbPreTestDataWorker.WarehouseService.GetDefaultWarehouse();
            var defaultCatalog = defaultWarehouse.WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");


            var us = MerchelloContext.Current.Services.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalog.Key, us);
            ((ServiceContext)MerchelloContext.Current.Services).ShipCountryService.Save(usCountry);


            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);

            var gwshipMethod2 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, usCountry, "Ground (Vary by Weight)");
            gwshipMethod2.RateTable.AddRow(0, 5, 5);
            gwshipMethod2.RateTable.AddRow(5, 7, 10); 
            gwshipMethod2.RateTable.AddRow(7, 25, 25);
            gwshipMethod2.RateTable.AddRow(25, 10000, 100);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod2);


            var taxProvider = MerchelloContext.Current.Gateways.Taxation.CreateInstance(Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);

            taxProvider.DeleteAllTaxMethods();

            var gwTaxMethod = taxProvider.CreateTaxMethod("US", 0);

            gwTaxMethod.TaxMethod.Provinces["WA"].PercentAdjustment = 8.7M;

            taxProvider.SaveTaxMethod(gwTaxMethod);

            BuildOrders();

        }

        public void BuildOrders()
        {
            var products = DbPreTestDataWorker.ProductService.GetAll().ToArray();
            var maxIndex = products.Count() - 1;

            var itemCount = MockDataMakerBase.NoWhammyStop.Next(11);


            for(var j = 0; j < InvoiceCount; j++)

            { 
            for (var i = 0; i < itemCount; i++)
            {
                CurrentCustomer.Basket().AddItem(products[MockDataMakerBase.NoWhammyStop.Next(maxIndex)], MockDataMakerBase.NoWhammyStop.Next(5));
            }
            CurrentCustomer.Basket().Save();



            // Customer enters the destination shipping address
            var destination = new Address()
            {
                Name = "Mindfly Web Design Studio",
                Address1 = "115 W. Magnolia St.",
                Address2 = "Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US"
            };

            // Assume customer selects billing address is same as shipping address
            CurrentCustomer.Basket().SalePreparation().SaveBillToAddress(destination);

            // --------------- ShipMethod Selection ----------------------------------

            // Package the shipments 
            //
            // This should return a collection containing a single shipment
            //
            var shipments = CurrentCustomer.Basket().PackageBasket(destination).ToArray();

            var shipment = shipments.First();

            var shipRateQuotes = shipment.ShipmentRateQuotes().ToArray();


            CurrentCustomer.Basket().SalePreparation().SaveShipmentRateQuote(shipRateQuotes.First());

            var paymentMethods = CurrentCustomer.Basket().SalePreparation().GetPaymentGatewayMethods();

            var paymentResult = CurrentCustomer.Basket().SalePreparation().AuthorizeCapturePayment(paymentMethods.FirstOrDefault());

                var invoice = paymentResult.Invoice;

                invoice.InvoiceDate = GetRandomDate();

                DbPreTestDataWorker.InvoiceService.Save(invoice);


            }
        }

        
        public void MakeProducts()
        {
            for (var i = 0; i < ProductCount; i++)
            {
                DbPreTestDataWorker.MakeExistingProduct(
                    true,
                    MockDataMakerBase.GetWeight(),
                    MockDataMakerBase.GetAmount());
            }
        }

        private static DateTime GetRandomDate()
        {
            var day = MockDataMakerBase.NoWhammyStop.Next(28);
            var month = MockDataMakerBase.NoWhammyStop.Next(12);
            const int Year = 2014;
            var hour = MockDataMakerBase.NoWhammyStop.Next(12);
            var min = MockDataMakerBase.NoWhammyStop.Next(59);

            var ampm = month % 2 == 0 ? "AM" : "PM";

            var random = string.Format("{0}/{1}/{2} {3}:{4} {5}", month, day, Year, hour, min, ampm);

            DateTime dt;

            if (!DateTime.TryParse(random, out dt))
            {
                return DateTime.Now;
            }

            return dt;
        }

    }
}
