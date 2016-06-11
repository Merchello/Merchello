namespace Merchello.Tests.PaymentProviders.PayPal.Api
{
    using System;
    using System.Net;

    using global::PayPal;

    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Providers.Payment.PayPal.Services;
    using Merchello.Tests.PaymentProviders.PayPal.TestHelpers;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class GeneralApiTests : PayPalTestBase
    {
        [Test]
        public void Can_Get_An_AccessToken()
        {
            //// Arrange
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 9999;

            var settings = ((PayPalApiService)PayPalApiService).Settings;

            var sdkConfig = settings.GetApiSdkConfig();

            //// Act
            var accessToken = new OAuthTokenCredential(settings.ClientId, settings.ClientSecret, sdkConfig.Result).GetAccessToken();

            //// Assert
            Console.WriteLine(accessToken);
            Assert.NotNull(accessToken);
        }

        [Test]
        public void Can_Create_An_APIContext()
        {
            //// Arrange
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 9999;

            var settings = ((PayPalApiService)PayPalApiService).Settings;

            var sdkConfig = settings.GetApiSdkConfig();

            var accessToken = new OAuthTokenCredential(settings.ClientId, settings.ClientSecret, sdkConfig.Result).GetAccessToken();

            //// Act
            var apiContext = new APIContext(accessToken);
            
            Assert.NotNull(apiContext);

        }
    }
}