namespace Merchello.Web.Workflow.InvoiceCreation.CheckoutManager
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains.InvoiceCreation.CheckoutManager;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Web.Discounts.Coupons;

    using Umbraco.Core;

    /// <summary>
    /// Adds any coupon discounts to invoice task.
    /// </summary>
    internal class AddCouponDiscountsToInvoiceTask : CheckoutManagerInvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// The <see cref="CouponManager"/>.
        /// </summary>
        private readonly Lazy<CouponManager> _couponManager = new Lazy<CouponManager>(() => CouponManager.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCouponDiscountsToInvoiceTask"/> class.
        /// </summary>
        /// <param name="checkoutManager">
        /// The checkout manager.
        /// </param>
        public AddCouponDiscountsToInvoiceTask(ICheckoutManagerBase checkoutManager)
            : base(checkoutManager)
        {
        }

        /// <summary>
        /// Gets the <see cref="CouponManager"/>.
        /// </summary>
        private CouponManager CouponOfferManager
        {
            get
            {
                return this._couponManager.Value;
            }
        }

        /// <summary>
        /// Attempts to add the coupons discounts to the invoice
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            // check if there are any coupon offers
            if (!this.CheckoutManager.Offer.OfferCodes.Any()) return Attempt<IInvoice>.Succeed(value);

            foreach (var code in this.CheckoutManager.Offer.OfferCodes)
            {
                var foundCoupon = this.CouponOfferManager.GetByOfferCode(code, this.CheckoutManager.Context.Customer);
                if (!foundCoupon.Success)
                {
                    continue;
                }

                var coupon = foundCoupon.Result;
                var clone = Extensions.CreateNewItemCacheLineItemContainer(value.Items.Where(x => x.LineItemType != LineItemType.Tax));
                var apply = coupon.TryApply(clone, this.CheckoutManager.Context.Customer).AsCouponRedemptionResult(coupon);
                if (apply.Success)
                {
                    this.CouponOfferManager.SafeAddCouponAttemptContainer<InvoiceLineItem>(value, apply, true);
                }
            }

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}