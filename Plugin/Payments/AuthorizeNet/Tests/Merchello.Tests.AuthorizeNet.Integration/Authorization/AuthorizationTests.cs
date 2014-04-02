using ClientDependency.Core;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.AuthorizeNet.Integration.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.AuthorizeNet.Integration.Authorization
{
    [TestFixture]
    public class AuthorizationTests : ProviderTestsBase
    {
        private IInvoice _invoice;

        [SetUp]
        public void Init()
        {

            var billTo = new Address()
            {
                Organization = "Mindfly Web Design Studios",
                Address1 = "114 W. Magnolia St. Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
                Email = "someone@mindfly.com",
                Phone = "555-555-5555"
            };

            // create an invoice
            var invoiceService = new InvoiceService();

            _invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            _invoice.SetBillingAddress(billTo);

            _invoice.Total = 120M;
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");
            
            // make up some line items
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 10, 1, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Product, "Item 2", "I2", 2, 40, extendedData);
            var l3 = new InvoiceLineItem(LineItemType.Shipping, "Shipping", "shipping", 1, 10M, extendedData);
            var l4 = new InvoiceLineItem(LineItemType.Tax, "Tax", "tax", 1, 10M, extendedData);

            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);
            _invoice.Items.Add(l3);
            _invoice.Items.Add(l4);

        }

        /// <summary>
        /// Testing Sandbox Authorize method
        /// </summary>
        [Test]
        public void Can_Authorize_A_Payment()
        {
           
        }
    }
}
