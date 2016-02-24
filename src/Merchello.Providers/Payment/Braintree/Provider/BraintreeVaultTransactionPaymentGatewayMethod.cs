namespace Merchello.Providers.Payment.Braintree.Provider
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Providers.Payment.Exceptions;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    using Constants = Merchello.Providers.Constants;

    /// <summary>
    /// Represents a BraintreeVaultTransactionPaymentGatewayMethod
    /// </summary>
    /// <remarks>
    /// This method assumes that the invoice is associated with a customer
    /// </remarks>
    [GatewayMethodUi("BrainTree.VaultTransaction")]
    [GatewayMethodEditor("BrainTree Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Braintree Payment Gateway Method Editors",
    "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.vault.authorizepayment.html",
    "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.vault.authorizecapturepayment.html",
    "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.voidpayment.html",
    "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.refundpayment.html",
    "~/App_Plugins/MerchelloProviders/views/dialogs/braintree.standard.capturepayment.html",
    true, true)]
    public class BraintreeVaultTransactionPaymentGatewayMethod : BraintreeVaultPaymentGatewayMethodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeVaultTransactionPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <param name="braintreeApiService">
        /// The braintree api service.
        /// </param>
        public BraintreeVaultTransactionPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IBraintreeApiService braintreeApiService) 
            : base(gatewayProviderService, paymentMethod, braintreeApiService)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the payment line authorize description.
        /// </summary>
        protected override string PaymentLineAuthorizeDescription { get; set; }

        /// <summary>
        /// Gets or sets the payment line authorize capture description.
        /// </summary>
        protected override string PaymentLineAuthorizeCaptureDescription { get; set; }

        /// <summary>
        /// Initializes the method.
        /// </summary>
        private void Initialize()
        {
            this.PaymentLineAuthorizeDescription = "To show record of Braintree Authorization";
            this.PaymentLineAuthorizeCaptureDescription = "Braintree Vault Transaction Authorized and Captured";
        }
    }
}