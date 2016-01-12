namespace Merchello.Web.Workflow.Checkout
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Builders;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Web.Pluggable;

    /// <summary>
    /// Represents a basket checkout manager.
    /// </summary>
    internal class BasketCheckoutManager : CheckoutManagerBase
    {

        #region Fields

        /// <summary>
        /// The <see cref="ICheckoutCustomerManager"/>.
        /// </summary>
        private Lazy<ICheckoutCustomerManager> _customerManager;

        /// <summary>
        /// The <see cref="ICheckoutExtendedManager"/>.
        /// </summary>
        private Lazy<ICheckoutExtendedManager> _extendedManager;

        /// <summary>
        /// The <see cref="ICheckoutOfferManager"/>.
        /// </summary>
        private Lazy<ICheckoutOfferManager> _offerManager;

        /// <summary>
        /// The <see cref="ICheckoutShippingManager"/>.
        /// </summary>
        private Lazy<ICheckoutShippingManager> _shippingManager;

        /// <summary>
        /// The <see cref="ICheckoutPaymentManager"/>.
        /// </summary>
        private Lazy<ICheckoutPaymentManager> _paymentManager;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutManager"/> class.
        /// </summary>
        /// <param name="checkoutContext">
        /// The checkout context.
        /// </param>
        public BasketCheckoutManager(ICheckoutContext checkoutContext)
            : base(checkoutContext)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutCustomerManager"/>.
        /// </summary>
        public override ICheckoutCustomerManager Customer
        {
            get
            {
                return _customerManager.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutExtendedManager"/>.
        /// </summary>
        public override ICheckoutExtendedManager Extended
        {
            get
            {
                return _extendedManager.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutOfferManager"/>.
        /// </summary>
        public override ICheckoutOfferManager Offer
        {
            get
            {
                return _offerManager.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutShippingManager"/>.
        /// </summary>
        public override ICheckoutShippingManager Shipping
        {
            get
            {
                return _shippingManager.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICheckoutPaymentManager"/>.
        /// </summary>
        public override ICheckoutPaymentManager Payment
        {
            get
            {
                return _paymentManager.Value;
            }
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            _customerManager = new Lazy<ICheckoutCustomerManager>(() => PluggableObjectHelper.GetInstance<BasketCheckoutCustomerManager>("BasketCheckoutCustomerManager", Context));
            //_extendedManager = new Lazy<ICheckoutExtendedManager>(() => PluggableObjectHelper.GetInstance<BasketE>());
            _offerManager = new Lazy<ICheckoutOfferManager>(() => PluggableObjectHelper.GetInstance<BasketCheckoutOfferManager>("BasketCheckoutOfferManager", Context));
            _shippingManager = new Lazy<ICheckoutShippingManager>(() => PluggableObjectHelper.GetInstance<BasketCheckoutShippingManager>("BasketCheckoutShippingManager", Context));
            _paymentManager = new Lazy<ICheckoutPaymentManager>(() => PluggableObjectHelper.GetInstance<BasketCheckoutPaymentManager>("BasketCheckoutPaymentManager", Context, InvoiceBuilder));
        }
    }
}