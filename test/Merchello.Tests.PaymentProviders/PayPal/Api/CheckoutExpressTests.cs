namespace Merchello.Tests.PaymentProviders.PayPal.Api
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Provider;
    using Merchello.Tests.PaymentProviders.PayPal.TestHelpers;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// The checkout express tests.
    /// </summary>
    [TestFixture]
    public class CheckoutExpressTests : PayPalTestBase
    {
        /// <summary>
        /// The <see cref="PayPalExpressCheckoutPaymentGatewayMethod"/>.
        /// </summary>
        private PayPalExpressCheckoutPaymentGatewayMethod _paymentMethod;

        ///// <summary>
        ///// The test fixture setup.
        ///// </summary>
        //public override void TestFixtureSetup()
        //{
        //    base.TestFixtureSetup();
        //    var gpSettings = new GatewayProviderSettings
        //    {
        //        Key =
        //                     Providers.Constants.PayPal
        //                     .GatewayProviderSettingsKey,
        //        ProviderTfKey =
        //                     Constants.TypeFieldKeys.GatewayProvider
        //                     .PaymentProviderKey,
        //        CreateDate = DateTime.Now,
        //        UpdateDate = DateTime.Now,
        //        Description = string.Empty,
        //        EncryptExtendedData = false,
        //        Name = "PayPal Checkout Express"
        //    };

        //    DbPreTestDataWorker.DeleteAllPaymentMethods();
        //    MerchelloContext.Current.Services.GatewayProviderService.Delete(gpSettings);

        //    var providers = MerchelloContext.Current.Gateways.Payment.GetAllActivatedProviders();
        //    PayPalPaymentGatewayProvider provider = null;

        //    if (providers.All(x => x.Key != Providers.Constants.PayPal.GatewayProviderSettingsKey))
        //    {


        //        var settings = TestHelper.GetPayPalProviderSettings();


        //        provider = new PayPalPaymentGatewayProvider(
        //            MerchelloContext.Current.Services.GatewayProviderService,
        //            gpSettings,
        //            MerchelloContext.Current.Cache.RuntimeCache);

        //        MerchelloContext.Current.Gateways.Payment.ActivateProvider(provider);
        //    }
        //    else
        //    {
        //        provider = (PayPalPaymentGatewayProvider)providers.FirstOrDefault(x => x.Key == Providers.Constants.PayPal.GatewayProviderSettingsKey);
        //    }

        //    var resource =
        //        provider.ListResourcesOffered()
        //            .FirstOrDefault(x => x.ServiceCode == Providers.Constants.PayPal.PaymentCodes.ExpressCheckout);
        //    if (resource != null)
        //    {

        //        this._paymentMethod = (PayPalExpressCheckoutPaymentGatewayMethod)provider.CreatePaymentMethod(resource, resource.Name, string.Empty);
        //    }

        //    _paymentMethod =
        //        new PayPalExpressCheckoutPaymentGatewayMethod(
        //            MerchelloContext.Current.Services.GatewayProviderService,
        //            gpSettings,
        //            PayPalApiService);
        //}

        [Test]
        public void Can_Get_ExpressCheckoutResponse()
        {
            //// Arrange
            var invoice = DbPreTestDataWorker.MakeExistingInvoices().First();
            

            //// Act

            //// Assert
        }
    }
}