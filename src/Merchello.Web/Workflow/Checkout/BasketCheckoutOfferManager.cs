namespace Merchello.Web.Workflow.Checkout
{
    using System;
    using System.Linq;

    using Merchello.Core.Checkout;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Web.Discounts.Coupons;

    using Umbraco.Core;

    /// <summary>
    /// Gets the basket offer manager.
    /// </summary>
    public class BasketCheckoutOfferManager : CheckoutOfferManagerBase
    {
        /// <summary>
        /// The <see cref="CouponManager"/>.
        /// </summary>
        private readonly Lazy<CouponManager> _couponManager = new Lazy<CouponManager>(() => CouponManager.Instance);


        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutOfferManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public BasketCheckoutOfferManager(ICheckoutContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Attempts to apply an offer to the the checkout.
        /// </summary>
        /// <param name="validateAgainst">
        /// The object to validate against
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
        /// <remarks>
        /// Custom offer types
        /// </remarks>
        internal override Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
        {
            return this.OfferCodes.Contains(offerCode) ?
                Attempt<IOfferResult<TConstraint, TAward>>.Fail(new OfferRedemptionException("This offer has already been added.")) :
                this.DoTryApplyOffer<TConstraint, TAward>(validateAgainst, offerCode);
        }

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
        private Attempt<IOfferResult<TConstraint, TAward>> DoTryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class
            where TAward : class
        {
            var foundOffer = GetCouponAttempt(offerCode);
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
        private Attempt<Coupon> GetCouponAttempt(string offerCode)
        {
            //// TODO RSS cache keys should not be hard coded within a class
            var cacheKey = string.Format("merchello.basksalepreparation.offercode.{0}", offerCode);
            return (Attempt<Coupon>)Context.Cache.GetCacheItem(cacheKey, () => _couponManager.Value.GetByOfferCode(offerCode, Context.Customer));
        }
    }
}