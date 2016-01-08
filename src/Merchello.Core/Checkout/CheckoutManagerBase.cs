namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The checkout manager base.
    /// </summary>
    public abstract class CheckoutManagerBase : CheckoutContextManagerBase, ICheckoutManagerBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutManagerBase"/> class.
        /// </summary>
        /// <param name="checkoutContext">
        /// The checkout Context.
        /// </param>
        protected CheckoutManagerBase(ICheckoutContext checkoutContext)
            : base(checkoutContext)
        {
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
        /// Gets the notification.
        /// </summary>
        public abstract ICheckoutNotificationManager Notification { get; }


        /// <summary>
        /// Gets the <see cref="IItemCache"/>
        /// </summary>
        public IItemCache ItemCache
        {
            get { return Context.ItemCache; }
        }

        /// <summary>
        /// Gets a clone of the ItemCache
        /// </summary>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal IItemCache CloneItemCache()
        {
            // The ItemCache needs to be cloned as line items may be altered while applying constraints
            return this.CloneItemCache(ItemCache);
        }

        /// <summary>
        /// Clones a <see cref="ILineItemContainer"/> as <see cref="IItemCache"/>
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal IItemCache CloneItemCache(ILineItemContainer container)
        {
            var clone = new ItemCache(Guid.NewGuid(), ItemCacheType.Backoffice);
            foreach (var item in container.Items)
            {
                clone.Items.Add(item.AsLineItemOf<ItemCacheLineItem>());
            }

            return clone;
        }

        /// <summary>
        /// Creates a new <see cref="ILineItemContainer"/> with filtered items.
        /// </summary>
        /// <param name="filteredItems">
        /// The line items.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemContainer"/>.
        /// </returns>
        internal ILineItemContainer CreateNewLineContainer(IEnumerable<ILineItem> filteredItems)
        {
            return LineItemExtensions.CreateNewItemCacheLineItemContainer(filteredItems);
        }
    }
}