namespace Merchello.Plugin.Payments.Braintree.Controllers
{
    using System;
    using System.Web.Http;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Plugin.Payments.Braintree.Factories;
    using Merchello.Plugin.Payments.Braintree.Provider;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// The Braintree API controller.
    /// </summary>
    [PluginController("MerchelloBraintree")]
    public class BraintreeApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="BraintreeGateway"/>.
        /// </summary>
        private readonly BraintreeGateway _gateway;

        /// <summary>
        /// The <see cref="BraintreeRequestFactory"/>.
        /// </summary>
        private readonly BraintreeRequestFactory _requestFactory;

        private readonly ICustomerService _customerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiController"/> class.
        /// </summary>
        public BraintreeApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public BraintreeApiController(IMerchelloContext merchelloContext)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");

            var provider = (BraintreePaymentGatewayProvider)merchelloContext.Gateways.Payment.GetProviderByKey(Braintree.Constants.GatewayProviderSettingsKey);

            if (provider  == null)
            {
                var ex = new NullReferenceException("The BraintreePaymentGatewayProvider could not be resolved.  The provider must be activiated");
                LogHelper.Error<BraintreeApiController>("BraintreePaymentGatewayProvider not activated.", ex);
                throw ex;
            }

            _customerService = merchelloContext.Services.CustomerService;

            _gateway = provider.ExtendedData.GetBrainTreeProviderSettings().AsBraintreeGateway();

            _requestFactory = new BraintreeRequestFactory();
        }

        /// <summary>
        /// Gets a client request token used in HTML form.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpGet]
        public string GetClientToken(Guid customerKey)
        {
            if (customerKey != Guid.Empty)
            {
                var customer = _customerService.GetAnyByKey(customerKey);

                // we want an anonymous token for anonymous customers
                if (customer.IsAnonymous) customerKey = Guid.Empty;
            }

            var request = _requestFactory.CreateClientTokenRequest(customerKey);

            return _gateway.ClientToken.generate(request);
        }
    }
}