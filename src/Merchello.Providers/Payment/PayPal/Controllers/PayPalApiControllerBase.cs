namespace Merchello.Providers.Payment.PayPal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Logging;
    using Merchello.Core.Services;
    using Merchello.Web.Pluggable;

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
        /// The <see cref="ICustomerContext"/>.
        /// </summary>
        private ICustomerContext _customerContext;

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
        /// Gets the <see cref="IInvoiceService"/>.
        /// </summary>
        protected IInvoiceService InvoiceService
        {
            get
            {
                return _merchelloContext.Services.InvoiceService;
            }
        }

        /// <summary>
        /// Gets the <see cref="IPaymentService"/>.
        /// </summary>
        protected IPaymentService PaymentService
        {
            get
            {
                return _merchelloContext.Services.PaymentService;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICustomerContext"/>.
        /// </summary>
        protected ICustomerContext CustomerContext
        {
            get
            {
                if (_customerContext == null)
                {
                    _customerContext = PluggableObjectHelper.GetInstance<CustomerContextBase>("CustomerContext", UmbracoContext);
                }
                return _customerContext;
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

        /// <summary>
        /// Gets the default extended log data.
        /// </summary>
        /// <returns>
        /// The <see cref="IExtendedLoggerData"/>.
        /// </returns>
        protected IExtendedLoggerData GetExtendedLoggerData()
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            logData.AddCategory("PayPal");

            return logData;
        }

    }
}