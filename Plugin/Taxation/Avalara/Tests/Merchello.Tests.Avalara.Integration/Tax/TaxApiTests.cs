namespace Merchello.Tests.Avalara.Integration.Tax
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting;

    using Merchello.Core.Models;
    using Merchello.Plugin.Taxation.Avalara;
    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;
    using Merchello.Plugin.Taxation.Avalara.Models.Tax;
    using Merchello.Plugin.Taxation.Avalara.Services;
    using Merchello.Tests.Avalara.Integration.TestBase;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class TaxApiTests : AvaTaxTestBase
    {
        const string JSON = @"{ ""DocDate"" : ""2011-05-11"",
                ""CustomerCode"": ""CUST1"",
                ""Addresses"": [{
                    ""AddressCode"": ""1"",
                    ""Line1"": ""435 Ericksen Avenue Northeast"",
                    ""Line2"" : ""#250"",
                    ""PostalCode"": ""98110""}],
                    ""Lines"" : [{
                    ""LineNo"": ""1"",
                    ""DestinationCode"": ""1"",
                    ""OriginCode"": ""1"",
                    ""Qty"": 1,
                    ""Amount"": 10}]}";
  
        /// <summary>
        /// Test verifies that the tax API can be pinged and we get a successful result
        /// </summary>
        [Test]
        public void Can_Ping_The_Tax_Api()
        {
            //// Arrange

            //// Act
            var result = AvaTaxService.Ping();

            //// Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.ResultCode, SeverityLevel.Success);
            Assert.IsTrue(result.TaxDetails.Any());
        }

        /// <summary>
        /// Test confirms that taxes can be estimated based off a lat long
        /// </summary>
        [Test]
        public void Can_Estimate_Tax_Based_On_Geo_Data()
        {
            //// Arrange
            // Mindfly lat/long
            var latitude = 48.751413M;
            var longitude = -122.478071M;
            var salesPrice = 10;

            //// Act
            var result = AvaTaxService.EstimateTax(latitude, longitude, salesPrice);

            //// Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.ResultCode, SeverityLevel.Success);
            Assert.IsTrue(result.TaxDetails.Any());
        }

        /// <summary>
        /// Test verifies the <see cref="TaxRequest"/> model is valid
        /// </summary>
        [Test]
        public void Can_Deserialize_Example_Json_To_TaxRequest()
        {
            //// Arrange
             
            //// Act
            var taxRequest = JsonConvert.DeserializeObject<TaxRequest>(JSON);
            taxRequest.DocCode = "INV-O1";

            //// Assert
            Assert.NotNull(taxRequest);
            Console.WriteLine(JsonConvert.SerializeObject(
                taxRequest, 
                Formatting.None, 
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
        }

        [Test]
        public void Can_Get_TaxResult_With_Deserialized_Example_Json()
        {
            //// Arrange
            var taxRequest = JsonConvert.DeserializeObject<TaxRequest>(JSON);
            taxRequest.DocCode = "INV-O1";
            //// Act
            var result = AvaTaxService.GetTax(taxRequest);

            //// Assert
            Assert.NotNull(result);

            Assert.AreEqual(result.ResultCode, SeverityLevel.Success);

        }

        /// <summary>
        /// Quick test to of the service API call
        /// </summary>
        [Test]
        public void Can_Get_Tax_Result_With_Simple_Mock_Data()
        {
            //// Arrange
            var requestUrl = ((AvaTaxService)AvaTaxService).GetApiUrl("tax", "get");

            //// Act
            var resultJson = ((AvaTaxService)AvaTaxService).GetResponse(requestUrl, JSON, RequestMethod.HttpPost);

            var result = JsonConvert.DeserializeObject<TaxResult>(resultJson);

            //// Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.ResultCode, SeverityLevel.Success);

        }

        /// <summary>
        /// Test confirms that a tax result can be retrieved from AvaTax using example data
        /// in their example library
        /// </summary>
        [Test]
        public void Can_Get_Tax_Result_With_IInvoice_Data()
        {
            //// Arrange
            var storeAddress = new Address()
                {
                    Name = "Mindfly, Inc.",
                    Address1 = "114 W. Magnolia St. Suite 300",
                    Locality = "Bellingham",
                    Region = "WA",
                    PostalCode = "98225",
                    CountryCode = "US"
                };

            var taxRequest = Invoice.AsTaxRequest(storeAddress.ToTaxAddress());
            taxRequest.DocCode = "INV-DT-" + Guid.NewGuid().ToString();
            Assert.NotNull(taxRequest);

            var result = AvaTaxService.GetTax(taxRequest);

            //// Assert
            Assert.NotNull(result);

            if (result.ResultCode != SeverityLevel.Success)
            if (result.Messages.Any()) 
                foreach(var message in result.Messages) Console.WriteLine(message.Details);

            Assert.AreEqual(result.ResultCode, SeverityLevel.Success);
        }
    }
}