namespace Merchello.Web.CheckoutManagers
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Web.Discounts.Coupons;

    using Umbraco.Core;

    /// <summary>
    /// Gets the basket offer manager.
    /// </summary>
    internal class BasketCheckoutOfferManager : CheckoutOfferManagerBase
    {
        /// <summary>
        /// The <see cref="CouponManager"/>.
        /// </summary>
        private readonly Lazy<CouponManager> _couponManager = new Lazy<CouponManager>(() => CouponManager.Instance);

        /// <summary>
        /// The <see cref="ICheckoutPaymentManager"/>.
        /// </summary>
        private readonly ICheckoutPaymentManager _paymentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutOfferManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="paymentManager">
        /// The <see cref="ICheckoutPaymentManager"/>.
        /// </param>
        public BasketCheckoutOfferManager(ICheckoutContext context, ICheckoutPaymentManager paymentManager)
            : base(context)
        {
            Mandate.ParameterNotNull(paymentManager, "paymentManager");

            this._paymentManager = paymentManager;
        }

        /// <summary>
        /// Attempts to redeem an offer to the sale.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedemptionResult{ILineItem}"/>.
        /// </returns>
        public override IOfferRedemptionResult<ILineItem> RedeemCouponOffer(string offerCode)
        {
            var couponAttempt = this.GetCouponAttempt(offerCode);
            if (!couponAttempt) return new CouponRedemptionResult(couponAttempt.Exception);

            var coupon = couponAttempt.Result;

            var validationItems = this._paymentManager.PrepareInvoice();
            var result = this.TryApplyOffer<ILineItemContainer, ILineItem>(LineItemExtensions.CreateNewItemCacheLineItemContainer(validationItems.Items.Where(x => x.LineItemType != LineItemType.Tax)), offerCode).AsCouponRedemptionResult(coupon);

            if (!result.Success) return result;

            // check if there are any previously added coupons and if so revalidate them with the new coupon added.
            // Use case:  First coupon added has the "not usable with other coupons constraint" and then a second coupon is added.
            // In this case the first coupon needs to be revalidated.  If the attempt to apply the coupon again fails, the one currently 
            // being added needs to fail.
            if (this.OfferCodes.Any())
            {
                // Now we have to revalidate any existing coupon offers to make sure the newly approved ones will still be valid.
                var clone = CheckoutManagerExtensions.CreateNewLineContainer(this.Context.ItemCache.Items.Where(x => x.LineItemType != LineItemType.Discount));

                this._couponManager.Value.SafeAddCouponAttemptContainer<ItemCacheLineItem>(clone, result);
                ICouponRedemptionResult redemption = new CouponRedemptionResult(result.Award, result.Messages);

                foreach (var oc in this.OfferCodes)
                {
                    redemption = this.DoTryApplyOffer<ILineItemContainer, ILineItem>(clone, oc).AsCouponRedemptionResult(coupon);
                    if (!redemption.Success)
                    {
                        if (redemption.Messages.Any()) result.AddMessage(redemption.Messages);

                        result.Exception = redemption.Exception;
                        result.Success = false;
                        break;
                    }

                    this._couponManager.Value.SafeAddCouponAttemptContainer<ItemCacheLineItem>(clone, result);
                }

                if (!redemption.Success) return redemption;
            }

            this.SaveOfferCode(offerCode);

            return result;
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
            var foundOffer = this.GetCouponAttempt(offerCode);
            if (!foundOffer.Success) return Attempt<IOfferResult<TConstraint, TAward>>.Fail(foundOffer.Exception);

            var coupon = foundOffer.Result;

            return coupon.TryApply<TConstraint, TAward>(validateAgainst, this.Context.Customer);
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
            return (Attempt<Coupon>)this.Context.Cache.GetCacheItem(cacheKey, () => this._couponManager.Value.GetByOfferCode(offerCode, this.Context.Customer));
        }
    }
}