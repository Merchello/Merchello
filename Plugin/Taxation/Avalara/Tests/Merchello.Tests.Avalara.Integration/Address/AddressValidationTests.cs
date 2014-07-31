namespace Merchello.Tests.Avalara.Integration.Address
{
    using System;

    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;
    using Merchello.Plugin.Taxation.Avalara.Services;

    using NUnit.Framework;

    [TestFixture]
    public class AddressValidationTests
    {
        private IAvaTaxService _avaTaxService;

        [TestFixtureSetUp]
        public void Init()
        {
            _avaTaxService = new AvaTaxService(TestHelper.GetAvaTaxProviderSettings());
        }

        /// <summary>
        /// Test verifies that the API request URL is constructed as expected
        /// </summary>
        [Test]
        public void Can_Construct_A_Valid_API_Request_Url()
        {
            //// Arrange
            const string Expected = "https://development.avalara.net/1.0/address/validate";

            //// Act
            var actual = ((AvaTaxService)_avaTaxService).GetApiUrl("address", "validate");

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


            var requestUrl = ((AvaTaxService)_avaTaxService).GetApiUrl("address", "validate") + "?" + address.AsApiQueryString();

            //// Act
            var response = ((AvaTaxService)_avaTaxService).GetResponse(requestUrl);

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
            var result = _avaTaxService.ValidateTaxAddress(address);

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.ResultCode == SeverityLevel.Success);
            Assert.IsNotNullOrEmpty(result.Address.Line1);
        }

    }
}