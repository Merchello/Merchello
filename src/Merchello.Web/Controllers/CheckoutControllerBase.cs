namespace Merchello.Web.Controllers
{
    using System.Web.Mvc;

    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// A base class for Checkout controllers.
    /// </summary>
    public abstract class CheckoutControllerBase : MerchelloUIControllerBase
    {
        /// <summary>
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </summary>
        private readonly CheckoutContextSettingsFactory _contextSettingsFactory;

        /// <summary>
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </summary>
        private ICheckoutManagerBase _checkoutManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutControllerBase"/> class.
        /// </summary>
        /// <param name="contextSettingsFactory">
        /// The checkout context settings factory.
        /// </param>
        protected CheckoutControllerBase(CheckoutContextSettingsFactory contextSettingsFactory)
        {
            Ensure.ParameterNotNull(contextSettingsFactory, "checkoutContextSettingsFactory");
            this._contextSettingsFactory = contextSettingsFactory;
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutContextManagerBase"/>.
        /// </summary>
        protected ICheckoutManagerBase CheckoutManager
        {
            get
            {
                if (this._checkoutManager == null)
                {
                    this._checkoutManager = this.Basket.GetCheckoutManager(this._contextSettingsFactory.Create());
                }

                return this._checkoutManager;
            }
        }

        /// <summary>
        /// A redirection for checkout attempts when basket is empty.
        /// </summary>
        /// <returns>
        /// The a redirection <see cref="ActionResult"/> for invalid checkout.
        /// </returns>
        protected virtual ActionResult InvalidCheckoutRedirect()
        {
            return Redirect("/");
        }

        /// <summary>
        /// Renders the invalid checkout stage partial view.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult InvalidCheckoutStagePartial()
        {
            return PartialView("InvalidCheckoutStage");
        }

        /// <summary>
        /// Gets the <see cref="GatewayMethodUiAttribute"/>.
        /// </summary>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <returns>
        /// The <see cref="GatewayMethodUiAttribute"/>.
        /// </returns>
        protected virtual GatewayMethodUiAttribute GetGatewayMethodUiAttribute(IPaymentMethod paymentMethod)
        {
            // We really need the payment gateway method so we can resolve the attribute
            var paymentGatewayMethod = this.GatewayContext.Payment.GetPaymentGatewayMethodByKey(paymentMethod.Key);

            // Get the attribute from the method so that we can resolve the controller.
            return paymentGatewayMethod.GetType().GetCustomAttribute<GatewayMethodUiAttribute>(false);
        }


        /// <summary>
        /// Gets the next <see cref="ICheckoutCustomerManager"/>.
        /// </summary>
        /// <param name="current">
        /// The current stage.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutWorkflowMarker"/>.
        /// </returns>
        protected virtual ICheckoutWorkflowMarker GetNextCheckoutWorkflowMarker(CheckoutStage current)
        {
            ICheckoutWorkflowMarker workflowMarker = null;

            switch (current)
            {
                case CheckoutStage.BillingAddress:
                    workflowMarker = new CheckoutWorkflowMarker
                        {
                            Previous = CheckoutStage.BillingAddress,
                            Current = CheckoutStage.ShippingAddress,
                            Next = CheckoutStage.ShipRateQuote
                        };
                    break;

                case CheckoutStage.ShippingAddress:
                    workflowMarker = new CheckoutWorkflowMarker
                        {
                            Previous = CheckoutStage.ShippingAddress,
                            Current = CheckoutStage.ShipRateQuote,
                            Next = CheckoutStage.PaymentMethod                    
                        };
                    break;

                case CheckoutStage.ShipRateQuote:
                    workflowMarker = new CheckoutWorkflowMarker
                        {
                            Previous = CheckoutStage.ShipRateQuote,
                            Current = CheckoutStage.PaymentMethod,
                            Next = CheckoutStage.Payment                    
                        };
                    break;

                case CheckoutStage.PaymentMethod:
                    workflowMarker = new CheckoutWorkflowMarker
                        {
                             Previous = CheckoutStage.PaymentMethod,
                             Current = CheckoutStage.Payment,
                             Next = CheckoutStage.None                
                        };
                    break;

                default:
                    workflowMarker = new CheckoutWorkflowMarker();
                    break;
            }

            return workflowMarker;
        }
    }
}