namespace Merchello.Tests.Braintree.Integration.Provider
{
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Plugin.Payments.Braintree.Models;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Tests.Braintree.Integration.TestHelpers;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class BraintreeProviderSettingsTests
    {
        /// <summary>
        /// Test checks JSON serialization of provider settings so that we can store in Provider ExtendedData
        /// </summary>
        [Test]
        public void Can_Serialize_ProviderSettings_To_CamelCased_Json()
        {
            //// Arrange
            var extendedData = new ExtendedDataCollection();
            var settings = TestHelper.GetBraintreeProviderSettings();
            Assert.NotNull(settings);
            Assert.AreEqual(EnvironmentType.Sandbox, settings.Environment);

            //// Act
            extendedData.SaveProviderSettings(settings);

            //// Assert
            Assert.IsTrue(extendedData.ContainsKey(Constants.ExtendedDataKeys.BraintreeProviderSettings));

        }

        [Test]
        public void Can_Deserialize_ProviderSettings_FromCamelCased_Json()
        {
            //// Arrange
            var extendedData = new ExtendedDataCollection();
            var settings = TestHelper.GetBraintreeProviderSettings();
            extendedData.SaveProviderSettings(settings);

            //// Act
            var json = extendedData.GetValue(Constants.ExtendedDataKeys.BraintreeProviderSettings);
            Assert.IsNotNullOrEmpty(json);

            var deserialized = JsonConvert.DeserializeObject<BraintreeProviderSettings>(json);
            Assert.NotNull(deserialized);

        }
    }
}