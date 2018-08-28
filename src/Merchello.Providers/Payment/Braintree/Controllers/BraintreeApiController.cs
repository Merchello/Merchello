namespace Merchello.Providers.Payment.Braintree.Controllers
{
    using System;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Provider;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// The Braintree API controller.
    /// </summary>
    [PluginController("Merchello")]
    public class BraintreeApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IBraintreeApiService"/>.
        /// </summary>
        private readonly IBraintreeApiService _braintreeApiService;

        /// <summary>
        /// The customer service.
        /// </summary>
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
            var provider = (BraintreePaymentGatewayProvider)merchelloContext.Gateways.Payment.GetProviderByKey(Constants.Braintree.GatewayProviderSettingsKey);

            if (provider  == null)
            {
                var ex = new NullReferenceException("The BraintreePaymentGatewayProvider could not be resolved.  The provider must be activiated");
                LogHelper.Error<BraintreeApiController>("BraintreePaymentGatewayProvider not activated.", ex);
                throw ex;
            }

            this._customerService = merchelloContext.Services.CustomerService;

            var settings = provider.ExtendedData.GetBrainTreeProviderSettings();

            this._braintreeApiService = new BraintreeApiService(merchelloContext, settings);
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
        public string GetClientRequestToken(Guid customerKey)
        {
            if (customerKey == Guid.Empty)
            {
                return this._braintreeApiService.Customer.GenerateClientRequestToken();
            } 

            var customer = this._customerService.GetAnyByKey(customerKey);

            return customer.IsAnonymous
                       ? this._braintreeApiService.Customer.GenerateClientRequestToken()
                       : this._braintreeApiService.Customer.GenerateClientRequestToken((ICustomer)customer);
        }

        /// <summary>
        /// Gets a client request token used in HTML form.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [HttpGet]
        public string GetClientRequestToken()
        {
            return this._braintreeApiService.Customer.GenerateClientRequestToken();
        }

    }
}