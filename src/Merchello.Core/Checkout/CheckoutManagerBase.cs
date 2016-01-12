namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Builders;
    using Merchello.Core.Models;

    /// <summary>
    /// The checkout manager base.
    /// </summary>
    public abstract class CheckoutManagerBase : CheckoutContextManagerBase, ICheckoutManagerBase
    {
        /// <summary>
        /// The invoice builder.
        /// </summary>
        private Lazy<BuildChainBase<IInvoice>> _invoiceBuilder; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutManagerBase"/> class.
        /// </summary>
        /// <param name="checkoutContext">
        /// The checkout Context.
        /// </param>
        protected CheckoutManagerBase(ICheckoutContext checkoutContext)
            : base(checkoutContext)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the checkout manager for customer information.
        /// </summary>
        public abstract ICheckoutCustomerManager Customer { get; }

        /// <summary>
        /// Gets the checkout extended manager for custom invoicing.
        /// </summary>
        public abstract ICheckoutExtendedManager Extended { get; }

        /// <summary>
        /// Gets the checkout manager for marketing offers.
        /// </summary>
        public abstract ICheckoutOfferManager Offer { get; }

        /// <summary>
        /// Gets the checkout manager for shipping.
        /// </summary>
        public abstract ICheckoutShippingManager Shipping { get; }

        /// <summary>
        /// Gets the payment.
        /// </summary>
        public abstract ICheckoutPaymentManager Payment { get; }

        /// <summary>
        /// Gets the invoice builder.
        /// </summary>
        /// <returns>
        /// The <see cref="BuildChainBase{IInvoice}"/>.
        /// </returns>
        protected virtual BuildChainBase<IInvoice> InvoiceBuilder
        {
            get
            {
                return this._invoiceBuilder.Value;
            }
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            this._invoiceBuilder = new Lazy<BuildChainBase<IInvoice>>(() => new CheckoutInvoiceBuilderChain(this));
        }
    }
}