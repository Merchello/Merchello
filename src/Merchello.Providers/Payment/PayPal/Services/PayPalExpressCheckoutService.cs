namespace Merchello.Providers.Payment.PayPal.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Events;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Factories;
    using Merchello.Providers.Payment.PayPal.Models;

    using global::PayPal.PayPalAPIInterfaceService;
    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a PayPalExpressCheckoutService.
    /// </summary>
    internal class PayPalExpressCheckoutService : PayPalApiServiceBase, IPayPalExpressCheckoutService
    {
        /// <summary>
        /// The default return URL.
        /// </summary>
        private const string DefaultReturnUrl = "{0}/umbraco/merchelloproviders/paypalexpress/success?invoiceKey={1}&paymentKey={2}";

        /// <summary>
        /// The default cancel URL.
        /// </summary>
        private const string DefaultCancelUrl = "{0}/umbraco/merchelloproviders/paypalexpress/success?invoiceKey={1}&paymentKey={2}";

        /// <summary>
        /// The version.
        /// </summary>
        private const string Version = "104.0";

        /// <summary>
        /// The Website URL.
        /// </summary>
        private readonly string _websiteUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressCheckoutService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalExpressCheckoutService(PayPalProviderSettings settings)
            : base(settings)
        {
            _websiteUrl = PayPalApiHelper.GetBaseWebsiteUrl();
        }

        /// <summary>
        /// Occurs before PayPalExpressCheckoutService is completely initialized.
        /// </summary>
        /// <remarks>
        /// Allows for overriding defaults
        /// </remarks>
        public static event TypedEventHandler<PayPalExpressCheckoutService, ObjectEventArgs<PayPalExpressCheckoutUrls>> InitializingExpressCheckout;


        /// <summary>
        /// Performs the setup for an express checkout.
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>.
        /// </param>
        /// <param name="payment">
        /// The <see cref="IPayment"/>
        /// </param>
        /// <returns>
        /// The <see cref="SetExpressCheckoutResponseType"/>.
        /// </returns>
        public ExpressCheckoutResponse SetExpressCheckout(IInvoice invoice, IPayment payment)
        {
            var urls = new PayPalExpressCheckoutUrls
                {
                    ReturnUrl = string.Format(DefaultReturnUrl, _websiteUrl, invoice.Key, payment.Key),
                    CancelUrl = string.Format(DefaultCancelUrl, _websiteUrl, invoice.Key, payment.Key)
                };

            // Raise the event so that the urls can be overridden
            InitializingExpressCheckout.RaiseEvent(new ObjectEventArgs<PayPalExpressCheckoutUrls>(urls), this);

            return SetExpressCheckout(invoice, payment, urls.ReturnUrl, urls.CancelUrl);
        }

        /// <summary>
        /// Performs the setup for an express checkout.
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>.
        /// </param>
        /// <param name="payment">
        /// The <see cref="IPayment"/>
        /// </param>
        /// <param name="returnUrl">
        /// The return URL.
        /// </param>
        /// <param name="cancelUrl">
        /// The cancel URL.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        protected virtual ExpressCheckoutResponse SetExpressCheckout(IInvoice invoice, IPayment payment, string returnUrl, string cancelUrl)
        {
            // Internal factory to build PaymentDetailsType from IInvoice
            var factory = new PayPalPaymentDetailsTypeFactory(new PayPalFactorySettings { WebsiteUrl = _websiteUrl });
            var paymentDetailsType = factory.Build(invoice, PaymentActionCodeType.SALE);

            // The API requires this be in a list
            var paymentDetailsList = new List<PaymentDetailsType>() { paymentDetailsType };

            // ExpressCheckout details
            var ecDetails = new SetExpressCheckoutRequestDetailsType()
                    {
                        ReturnURL = returnUrl,
                        CancelURL = cancelUrl,
                        PaymentDetails = paymentDetailsList
                    };

            // The ExpressCheckoutRequest
            var request = new SetExpressCheckoutRequestType
                    {
                        Version = Version,
                        SetExpressCheckoutRequestDetails = ecDetails
                    };

            // Crete the wrapper for Express Checkout
            var wrapper = new SetExpressCheckoutReq { SetExpressCheckoutRequest = request };

            // We are getting the SDK authentication values from the settings saved through the back office
            // and stored in ExtendedData.  This returns an Attempt<PayPalSettings> so that we can 
            // assert that the values have been entered into the back office.
            var attempSdk = Settings.GetExpressCheckoutSdkConfig();

            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("GatewayProviders");
            logData.AddCategory("PayPal");

            if (attempSdk.Success)
            {
                var service = new PayPalAPIInterfaceServiceService(attempSdk.Result);
                try
                {
                    EnsureSslTslChannel();
                    var response = service.SetExpressCheckout(wrapper);

                    return new ExpressCheckoutResponse
                               {
                                   Ack = response.Ack,
                                   Token = response.Token,
                                   Version = response.Version,
                                   Build = response.Build,
                                   ErrorTypes = response.Errors,
                                   RedirectUrl = GetRedirectUrl(response.Token)
                               };
                }
                catch (Exception ex)
                {
                    MultiLogHelper.Error<PayPalExpressCheckoutService>("Failed to get response from PayPalAPIInterfaceServiceService", ex, logData);

                    return new ExpressCheckoutResponse { Ack = AckCodeType.CUSTOMCODE };
                }
            }

            MultiLogHelper.Error<PayPalExpressCheckoutService>("Could not load SDK settings", attempSdk.Exception, logData);

            throw attempSdk.Exception;
        }

        /// <summary>
        /// Gets the redirection URL for PayPal.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// This value will be put into the <see cref="IPayment"/>'s <see cref="ExtendedDataCollection"/>
        /// </remarks>
        private string GetRedirectUrl(string token)
        {
            return string.Format("https://www.{0}paypal.com/cgi-bin/webscr?cmd=_express-checkout&token={1}", Settings.Mode == PayPalMode.Live ? string.Empty : "sandbox.", token);
        }
    }
}