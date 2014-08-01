namespace Merchello.Tests.Avalara.Integration.Address
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Plugin.Taxation.Avalara;
    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;
    using Merchello.Plugin.Taxation.Avalara.Services;
    using Merchello.Tests.Avalara.Integration.TestBase;

    using NUnit.Framework;

    [TestFixture]
    public class AddressValidationTests : AvaTaxTestBase
    {
        
        /// <summary>
        /// Test verifies that the API request URL is constructed as expected
        /// </summary>
        [Test]
        public void Can_Construct_A_Valid_API_Request_Url()
        {
            //// Arrange
            const string Expected = "https://development.avalara.net/1.0/address/validate";

            //// Act
            var actual = ((AvaTaxService)this.AvaTaxService).GetApiUrl("address", "validate");

            //// Assert
            Assert.AreEqual(Expected, actual, "Urls were not equal");
        }

        /// <summary>
        /// Quick test to see if I can get a valid JSON object back from the API
        /// </summary>
        [Test]
        public void Can_Get_A_Result_From_The_Api()
        {
            //// Arrange
            var address = new ValidatableAddress()
                              {
                                  Line1 = "114 W. Magnolia St.",
                                  Line2 = "Suite 300",
                                  City = "Bellingham",
                                  Region = "WA",
                                  PostalCode = "98225",
                                  Country = "US"
                              };


            var requestUrl = ((AvaTaxService)this.AvaTaxService).GetApiUrl("address", "validate") + "?" + address.AsApiQueryString();

            //// Act
            var response = ((AvaTaxService)this.AvaTaxService).GetResponse(requestUrl);

            //// Assert
            Assert.NotNull(response);
            Console.Write(response);
        }

        /// <summary>
        /// Test confirms that an address can be validated against the address API
        /// </summary>
        [Test]
        public void Can_Validate_A_Address_Using_The_Address_Validate_Api()
        {
            //// Arrange
            var address = new ValidatableAddress()
            {
                Line1 = "114 W. Magnolia St.",
                Line2 = "Suite 300",
                City = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                Country = "US"
            };

            //// Act
            var result = this.AvaTaxService.ValidateTaxAddress(address);

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.ResultCode == SeverityLevel.Success);
            Assert.IsNotNullOrEmpty(result.Address.Line1);
        }

        /// <summary>
        /// Test verifies that the billing address from an invoice can be validated
        /// </summary>
        [Test]
        public void Can_Validate_Billing_Address_From_Invoice()
        {
            //// Arrange
            var address = Invoice.GetBillingAddress().ToValidatableAddress();
            var taxAddress = address.ToTaxAddress();

            //// Act
            var result = this.AvaTaxService.ValidateTaxAddress(address);

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.ResultCode == SeverityLevel.Success);
            Assert.IsNotNullOrEmpty(result.Address.Line1);
        }

        /// <summary>
        /// Test verifies that a shipping address can be validated from shipment line item
        /// </summary>
        [Test]
        public void Can_Validate_Shipping_Address_From_Invoice_Line_Item()
        {
            //// Arrange
            var shippableItems = Invoice.ShippingLineItems();
            var shipment = shippableItems.FirstOrDefault().ExtendedData.GetShipment<OrderLineItem>();
            var origin = shipment.GetOriginAddress();
            var destination = shipment.GetDestinationAddress();

            //// Act
            var resultOrigin = AvaTaxService.ValidateTaxAddress(origin.ToValidatableAddress());
            var resultDestination = AvaTaxService.ValidateTaxAddress(destination.ToValidatableAddress());

            //// Assert
            Assert.NotNull(resultOrigin);
            Assert.IsTrue(resultOrigin.ResultCode == SeverityLevel.Success);
            Assert.NotNull(resultDestination);
            Assert.IsTrue(resultDestination.ResultCode == SeverityLevel.Success);
        }
    }
}