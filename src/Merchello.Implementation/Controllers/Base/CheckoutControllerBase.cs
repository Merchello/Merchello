namespace Merchello.Implementation.Controllers.Base
{
    using Merchello.Core.Checkout;
    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;
    using Merchello.Web;
    using Merchello.Web.Mvc;

    using Umbraco.Core;

    /// <summary>
    /// A base class for Checkout controllers.
    /// </summary>
    public abstract class CheckoutControllerBase : MerchelloSurfaceController
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
            Mandate.ParameterNotNull(contextSettingsFactory, "checkoutContextSettingsFactory");
            this._contextSettingsFactory = contextSettingsFactory;
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutContextManagerBase"/>.
        /// </summary>
        protected ICheckoutManagerBase CheckoutManager
        {
            get
            {
                if (_checkoutManager == null)
                {
                    _checkoutManager = Basket.GetCheckoutManager(_contextSettingsFactory.Create());
                }

                return _checkoutManager;
            }
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