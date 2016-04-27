namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Net.Http;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// A base controller for receiving responses from PayPal's REST payment API.
    /// </summary>
    public abstract class PayPalApiControllerBase : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiControllerBase"/> class.
        /// </summary>
        protected PayPalApiControllerBase()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiControllerBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        protected PayPalApiControllerBase(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// Gets the <see cref="IGatewayContext"/>.
        /// </summary>
        protected IGatewayContext GatewayContext
        {
            get
            {
                return _merchelloContext.Gateways;
            }
        }

        /// <summary>
        /// Handles a successful return from PayPal
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public abstract HttpResponseMessage Success(Guid invoiceKey, Guid paymentKey, string token, string payerId);

        /// <summary>
        /// Handles a cancellation response from PayPal
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public abstract HttpResponseMessage Cancel(Guid invoiceKey, Guid paymentKey, string token, string payerId = null);
    }
}