namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Services;

    /// <summary>
    /// A base class for CheckoutCustomerManagers.
    /// </summary>
    public abstract class CheckoutCustomerManagerBase : CheckoutCustomerDataManagerBase, ICheckoutCustomerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutCustomerManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutCustomerManagerBase(ICheckoutContext context)
            : base(context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Saves the billing address.
        /// </summary>
        /// <param name="billToAddress">
        /// The bill to address.
        /// </param>
        public abstract void SaveBillToAddress(IAddress billToAddress);

        /// <summary>
        /// Saves the shipping.
        /// </summary>
        /// <param name="shipToAddress">
        /// The shipping address.
        /// </param>
        public abstract void SaveShipToAddress(IAddress shipToAddress);

        /// <summary>
        /// Gets the billing address.
        /// </summary>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public abstract IAddress GetBillToAddress();

        /// <summary>
        /// Gets the shipping to address.
        /// </summary>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public abstract IAddress GetShipToAddress();

        /// <summary>
        /// Clears the customer data on context reset.
        /// </summary>
        protected virtual void Reset()
        {
            Context.Customer.ExtendedData.RemoveValue(Core.Constants.ExtendedDataKeys.ShippingDestinationAddress);
            Context.Customer.ExtendedData.RemoveValue(Core.Constants.ExtendedDataKeys.BillingAddress);
            SaveCustomer();
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            if (Context.IsNewVersion) this.Reset();
        }
    }
}