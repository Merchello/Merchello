using Merchello.Tests.Base.TestHelpers;

namespace Merchello.Tests.IntegrationTests.Invoices
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Core.Strategies.Packaging;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.IntegrationTests.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class InvoiceTests : MerchelloAllInTestBase
    {
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllInvoices();
            DbPreTestDataWorker.DeleteAllShipCountries();
        }

        public void Can_Create_A_Customer_Invoice_And_Order()
        {
            // Adding the shipmethod is typically done in the back office through the UI.
            // Interested in the use case to dynamically add theses?
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var defaultCatalogKey = Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey;

            // this would have to be done through the back office as it uses an internal service
            var us = MerchelloContext.Current.Services.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalogKey, us);
            ((ServiceContext)MerchelloContext.Current.Services).ShipCountryService.Save(usCountry);

            // we can use this later.
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.GetProviderByKey(key);

            // again usually done in the back office
            if (!rateTableProvider.ShipMethods.Any())
            { 
                // creates the rate table for ship rate quotes
                var gwShipmeMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, usCountry, "Ground (Vary by Weight)");
                gwShipmeMethod.RateTable.AddRow(0, 10, 5);
                gwShipmeMethod.RateTable.AddRow(10, 15, 10); // total weight should be 10M so we should hit this tier
                gwShipmeMethod.RateTable.AddRow(15, 25, 25);
                gwShipmeMethod.RateTable.AddRow(25, 10000, 100);
                rateTableProvider.SaveShippingGatewayMethod(gwShipmeMethod);
            }



            // Get the persisted customer
            const string loginName = "rusty";

            var customerService = MerchelloContext.Current.Services.CustomerService;

            var customer = customerService.GetByLoginName(loginName)
                           ?? customerService.CreateCustomerWithKey(loginName, "Rusty", "Swayne", "rusty@mindfly.com");

            // I'll use this for billing and shipping
            var billingAddress = new Address()
                                     {
                                         Name = "Mindfly Web Design Studio",
                                         Address1 = "114 W. Magnolia St. Suite 300",
                                         Locality = "Bellingham",
                                         Region = "WA",
                                         PostalCode = "98225",
                                         CountryCode = "US"
                                     };

            // Most of the time this information is brought in from the IProductVariant - but the idea is you can
            // describe things on the fly
            var extendedData = new ExtendedDataCollection();
            // this is used to determine where a shipment originates
            extendedData.SetValue(Constants.ExtendedDataKeys.WarehouseCatalogKey, defaultCatalogKey.ToString());
            // items set to shippable
            extendedData.SetValue(Constants.ExtendedDataKeys.TrackInventory, "false");
            extendedData.SetValue(Constants.ExtendedDataKeys.Shippable, "true");
            extendedData.SetValue(Constants.ExtendedDataKeys.Weight, "1.25");
            extendedData.SetValue(Constants.ExtendedDataKeys.CurrencyCode, "USD");

            var item = new InvoiceLineItem(LineItemType.Product, "My product", "mySku", 2, 10M, extendedData); 

            var invoiceService = MerchelloContext.Current.Services.InvoiceService;

            var invoice = invoiceService.CreateInvoice(Constants.DefaultKeys.InvoiceStatus.Unpaid);
            // I'd say we need to add a parameter to the service so we don't have to do this
            // http://issues.merchello.com/youtrack/issue/M-434
            ((Invoice)invoice).CustomerKey = customer.Key;

            // The version key is useful in some cases to invalidate shipping quotes or taxation calculations
            invoice.VersionKey = Guid.NewGuid();

            invoice.Items.Add(item);

            // at this point the invoice is not saved and we don't have an invoice number
            // however, we may want to quote shipping so we need a shipment

            // Shipment Statuses are new in 1.5.0 
            var warehouse = MerchelloContext.Current.Services.WarehouseService.GetDefaultWarehouse();

            var shipmentStatus =
                MerchelloContext.Current.Services.ShipmentService.GetShipmentStatusByKey(
                    Constants.DefaultKeys.ShipmentStatus.Quoted);

            // since we know all the items in the invoice will be shipped we don't need to filter
            var shipment = new Shipment(shipmentStatus, warehouse.AsAddress(), billingAddress, invoice.Items);

            // since we already know the shipping provider we want from above we can do 
            var quotes = rateTableProvider.QuoteShippingGatewayMethodsForShipment(shipment);

            // if we wanted Merchello to get quotes from all shipping providers we'd do the following
            // var quotes = shipment.ShipmentRateQuotes();

            if (quotes.Any())
            {
                // this check makes certain a quote was returned.  For example if the collection of items was outside the allowable
                // weight range, the provider would not return a quote.

                // Add the first quote to the invoice.

                invoice.Items.Add(quotes.FirstOrDefault().AsLineItemOf<InvoiceLineItem>());
            }

            // you do need to update the total ... this is usually done in the InvoiceBuilder in 
            // instantiated by a SalesPreparation sub class
            var charges = invoice.Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => x.TotalPrice);
            var discounts = invoice.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice);

            // total the invoice
            decimal converted;
            invoice.Total = decimal.TryParse((charges - discounts).ToString(CultureInfo.InvariantCulture), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out converted) ? converted : 0;
               
            // Now we save the invoice since we have to have a real record of something to collect a payment against
            // This also generates the invoice number
            invoiceService.Save(invoice);

            Console.WriteLine(invoice.InvoiceNumber);

            // cash payment method
            var cashProvider = MerchelloContext.Current.Gateways.Payment.GetProviderByKey(Constants.ProviderKeys.Payment.CashPaymentProviderKey);

            if (cashProvider != null)
            {
                var cash = cashProvider.PaymentMethods.FirstOrDefault(); // default install has a single payment method "Cash"

                // I usually Authorize a cash payment if taken online since we don't really see the money.  Capture is used 
                // when the money comes in.  In your inquiry, it looks like you are assuming the money is in hand at the 
                // time of the purchase, so we'll use AuthorizeCapture straight away.

                var attempt = invoice.AuthorizeCapturePayment(cash.Key);

                if (! attempt.Payment.Success)
                {
                    // handle the error
                }

                // otherwise you'll notice
                var approved = attempt.ApproveOrderCreation; // equals true

                // the order will be automatically created by the event handler in Merchello.Core.Gateways.GatewayEvents

                // however in this test I don't have the event wired up so I have to do it manuall
                if (approved)
                {
                   var order = invoice.PrepareOrder();
                   MerchelloContext.Current.Services.OrderService.Save(order);

                    var items = order.Items;
                }
            }
            // Cash provider is not active

       

        }

        /// <summary>
        /// Test verifies that shipment line items stores extended data values
        /// </summary>
        [Test]
        public void Can_Prove_Serialized_Shipment_Stores_ExtendedDataCollection_In_LineItems()
        {
            //// Arrange
            var invoice = MockInvoiceDataMaker.GetMockInvoiceForTaxation();
            Assert.NotNull(invoice, "Invoice is null");

            //// Act
            var shipmentLineItems = invoice.ShippingLineItems().ToArray();
            Assert.IsTrue(shipmentLineItems.Any());
            var shipment = shipmentLineItems.First().ExtendedData.GetShipment<OrderLineItem>();

            //// Assert
            Assert.NotNull(shipment, "Shipment was null");
            Assert.IsTrue(shipment.Items.Any(), "Shipment did not contain any line items");
            Assert.IsFalse(shipment.Items.All(x => x.ExtendedData.IsEmpty), "Extended data in one or more line items was empty");
            Assert.IsTrue(shipment.Items.Any(x => x.ExtendedData.GetTaxableValue()), "Shipment does not contain any taxable items");

            foreach (var item in shipment.Items)
            {
                Assert.IsTrue(invoice.Items.Any(x => x.Sku == item.Sku), "No item exists for sku " + item.Sku);
            }
        } 
    }
}