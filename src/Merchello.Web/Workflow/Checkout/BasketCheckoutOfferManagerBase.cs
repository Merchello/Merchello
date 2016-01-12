namespace Merchello.Web.Workflow.Checkout
{
    using System;

    using Merchello.Core.Checkout;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Web.Discounts.Coupons;

    using Umbraco.Core;

    /// <summary>
    /// A base class for basket checkout offer manager.
    /// </summary>
    internal abstract class BasketCheckoutOfferManagerBase : CheckoutOfferManagerBase
    {
        /// <summary>
        /// The <see cref="CouponManager"/>.
        /// </summary>
        private readonly Lazy<CouponManager> _couponManager = new Lazy<CouponManager>(() => CouponManager.Instance);


        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutOfferManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ICheckoutContext"/>.
        /// </param>
        protected BasketCheckoutOfferManagerBase(ICheckoutContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets the <see cref="CouponManager"/>.
        /// </summary>
        protected CouponManager CouponManager
        {
            get
            {
                return _couponManager.Value;
            }
        }

        /// <summary>
        /// Attempts to add a coupon offer to the sale.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="ICouponRedemptionResult"/>.
        /// </returns>
        public abstract ICouponRedemptionResult RedeemCouponOffer(string offerCode);

        /// <summary>
        /// Does the actual work of attempting to apply the offer
        /// </summary>
        /// <param name="validateAgainst">
        /// The validate against.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        protected virtual Attempt<IOfferResult<TConstraint, TAward>> DoTryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class
            where TAward : class
        {
            var foundOffer = this.GetCouponAttempt(offerCode);
            if (!foundOffer.Success) return Attempt<IOfferResult<TConstraint, TAward>>.Fail(foundOffer.Exception);

            var coupon = foundOffer.Result;

            return coupon.TryApply<TConstraint, TAward>(validateAgainst, Context.Customer);
        }

        /// <summary>
        /// Gets the coupon attempt.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        /// <remarks>
        /// Caches to the request cache
        /// </remarks>
        protected virtual Attempt<Coupon> GetCouponAttempt(string offerCode)
        {
            var cacheKey = Core.Cache.CacheKeys.GetCheckoutOfferKey(Context.VersionKey, offerCode);
            return (Attempt<Coupon>)Context.Cache.GetCacheItem(cacheKey, () => _couponManager.Value.GetByOfferCode(offerCode, Context.Customer));
        }
    }
}