namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The <see cref="ICheckoutContext"/> event args.
    /// </summary>
    public class CheckoutContextEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutContextEventArgs"/> class.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        public CheckoutContextEventArgs(ICustomerBase customer, IItemCache itemCache)
        {
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(itemCache, "itemCache");

            this.Customer = customer;
            this.ItemCache = ItemCache;
        }

        /// <summary>
        /// Gets the customer.
        /// </summary>
        public ICustomerBase Customer { get; private set; }

        /// <summary>
        /// Gets the item cache.
        /// </summary>
        public IItemCache ItemCache { get; private set; }

        /// <summary>
        /// Gets the version key.
        /// </summary>
        public virtual Guid VersionKey
        {
            get
            {
                return ItemCache.VersionKey;
            }
        }
    }
}