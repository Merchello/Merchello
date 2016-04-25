namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.PayPal.Provider;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// The PayPal API controller.
    /// </summary>
    [PluginController("MerchelloPayments")]
    public class PayPalApiController : UmbracoApiController
    {
        /// <summary>
        /// Merchello context
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The PayPal payment processor.
        /// </summary>
        private readonly PayPalPaymentProcessor _processor;
		
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiController"/> class.
        /// </summary>
        public PayPalApiController()
            : this(MerchelloContext.Current)
        {
        }
		
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public PayPalApiController(IMerchelloContext merchelloContext)
        {
            if (merchelloContext == null) throw new ArgumentNullException(nameof(merchelloContext));

	        var providerKey = new Guid(Constants.PayPal.GatewayProviderKey);
            var provider = (PayPalPaymentGatewayProvider)merchelloContext.Gateways.Payment.GetProviderByKey(providerKey);

            if (provider  == null)
            {
                var ex = new NullReferenceException("The PayPalPaymentGatewayProvider could not be resolved.  The provider must be activiated");
                LogHelper.Error<PayPalApiController>("PayPalPaymentGatewayProvider not activated.", ex);
                throw ex;
            }

            this._merchelloContext = merchelloContext;
            this._processor = new PayPalPaymentProcessor(provider.ExtendedData.GetPayPalProviderSettings());
        }
    }
}