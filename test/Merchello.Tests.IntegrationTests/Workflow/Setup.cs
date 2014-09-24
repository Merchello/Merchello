using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Workflow
{
    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
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

        private const int InvoiceCount = 50;

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

        public void clean()
        {
            DbPreTestDataWorker.DeleteAllOrders();
            
        }

        public void ReportTestsSetup()
        {
            DbPreTestDataWorker.DeleteAllAnonymousCustomers();
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
            var addresses = MockDataMakerBase.FakeAddresses().Where(x => x.CountryCode == "US").ToArray();

            var maxProductIndex = products.Count() -1;
            var maxAddressIndex = addresses.Count();

            var itemCount = MockDataMakerBase.NoWhammyStop.Next(11);

            var invoiceDate = DateTime.Today.AddDays(-1 * InvoiceCount);

            for(var j = 0; j < InvoiceCount; j++)

            { 
            for (var i = 0; i < itemCount; i++)
            {
                CurrentCustomer.Basket().AddItem(products[MockDataMakerBase.NoWhammyStop.Next(maxProductIndex)], MockDataMakerBase.NoWhammyStop.Next(5));
            }
            CurrentCustomer.Basket().Save();



            // Customer enters the destination shipping address
                var destination = addresses[MockDataMakerBase.NoWhammyStop.Next(maxAddressIndex)];

            // Assume customer selects billing address is same as shipping address
            CurrentCustomer.Basket().SalePreparation().SaveBillToAddress(destination);

            // --------------- ShipMethod Selection ----------------------------------

            // Package the shipments 
            //
            // This should return a collection containing a single shipment
            //
            var shipments = CurrentCustomer.Basket().PackageBasket(destination).ToArray();

            var shipment = shipments.FirstOrDefault();

                if (shipment != null)
                {
                    var shipRateQuotes = shipment.ShipmentRateQuotes().ToArray();

                    CurrentCustomer.Basket().SalePreparation().SaveShipmentRateQuote(shipRateQuotes.First());
                }

                var paymentMethods = CurrentCustomer.Basket().SalePreparation().GetPaymentGatewayMethods();

                
            IPaymentResult paymentResult;
            paymentResult = invoiceDate.Month < DateTime.Now.AddDays(-30).Month
                    ? 
                      CurrentCustomer.Basket()
                          .SalePreparation()
                          .AuthorizeCapturePayment(paymentMethods.FirstOrDefault())
                    : 
                      CurrentCustomer.Basket()
                          .SalePreparation()
                          .AuthorizePayment(paymentMethods.FirstOrDefault());

                var invoice = paymentResult.Invoice;

                invoice.InvoiceDate = invoiceDate;

                invoiceDate = invoiceDate.AddDays(1);

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
            var day = MockDataMakerBase.NoWhammyStop.Next(30);
            var year = MockDataMakerBase.NoWhammyStop.Next(2012, DateTime.Now.Year);            
            var month = MockDataMakerBase.NoWhammyStop.Next(12);

            if (year == DateTime.Now.Year && month > DateTime.Now.Month) return DateTime.Now;

            if (month == 2 && day > 28) day = 28;
            
            var hour = MockDataMakerBase.NoWhammyStop.Next(12);
            var min = MockDataMakerBase.NoWhammyStop.Next(59);

            var ampm = month % 2 == 0 ? "AM" : "PM";

            var random = string.Format("{0}/{1}/{2} {3}:{4} {5}", month, day, year, hour, min, ampm);

            DateTime dt;

            if (!DateTime.TryParse(random, out dt))
            {
                return DateTime.Now;
            }

            return dt;
        }

    }
}
