﻿namespace Merchello.Web.Workflow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Web.Discounts.Coupons;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Cache;

    /// <summary>
    /// Represents the basket sale preparation.
    /// </summary>
    public class BasketSalePreparation : SalePreparationBase, IBasketSalePreparation
    {
        /// <summary>
        /// The <see cref="CouponManager"/>.
        /// </summary>
        private readonly Lazy<CouponManager> _couponManager = new Lazy<CouponManager>(() => CouponManager.Instance);

        /// <summary>
        /// The _request cache.
        /// </summary>
        private readonly ICacheProvider _requestCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketSalePreparation"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        internal BasketSalePreparation(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer)
            : base(merchelloContext, itemCache, customer)
        {
            _requestCache = merchelloContext.Cache.RequestCache;
        }

        /// <summary>
        /// Attempts to authorize a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            var result = base.AuthorizePayment(paymentGatewayMethod, args);

            if (result.Payment.Success) Customer.Basket().Empty();
            
            return result;
        }

        /// <summary>
        /// Authorizes and Captures a Payment 
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            var result = base.AuthorizeCapturePayment(paymentGatewayMethod, args);

            if (result.Payment.Success) Customer.Basket().Empty();

            return result;
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
        public ICouponRedemptionResult RedeemCouponOffer(string offerCode)
        {
            var couponAttempt = this.GetCouponAttempt(offerCode);
            if (!couponAttempt) return new CouponRedemptionResult(couponAttempt.Exception);

            var coupon = couponAttempt.Result;

            var validationItems = this.PrepareInvoice();
            var result = TryApplyOffer<ILineItemContainer, ILineItem>(LineItemExtensions.CreateNewBackOfficeLineItemContainer(validationItems.Items.Where(x => x.LineItemType != LineItemType.Tax)), offerCode).AsCouponRedemptionResult(coupon);

            if (!result.Success) return result;

            // check if there are any previously added coupons and if so revalidate them with the new coupon added.
            // Use case:  First coupon added has the "not usable with other coupons constraint" and then a second coupon is added.
            // In this case the first coupon needs to be revalidated.  If the attempt to apply the coupon again fails, the one currently 
            // being added needs to fail.
            if (OfferCodes.Any())
            {
                // Now we have to revalidate any existing coupon offers to make sure the newly approved ones will still be valid.
                var clone = this.CreateNewLineContainer(ItemCache.Items.Where(x => x.LineItemType != LineItemType.Discount));

                _couponManager.Value.SafeAddCouponAttemptContainer<ItemCacheLineItem>(clone, result);
                ICouponRedemptionResult redemption = new CouponRedemptionResult(result.Award, result.Messages);

                foreach (var oc in OfferCodes)
                {
                    redemption = DoTryApplyOffer<ILineItemContainer, ILineItem>(clone, oc).AsCouponRedemptionResult(coupon);
                    if (!redemption.Success)
                    {
                        if (redemption.Messages.Any()) result.AddMessage(redemption.Messages);

                        result.Exception = redemption.Exception;
                        result.Success = false;
                        break;
                    }

                    _couponManager.Value.SafeAddCouponAttemptContainer<ItemCacheLineItem>(clone, result); 
                }

                if (!redemption.Success) return redemption;
            }

            this.SaveOfferCode(offerCode);

            return result;
        }

        /// <summary>
        /// Attempts to apply an offer to the the checkout.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="autoAddOnSuccess">
        /// A value indicating whether or not the reward should be added to the <see cref="ILineItemContainer"/> on success
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        /// <remarks>
        /// TODO move this to an InvoiceChainTask
        /// </remarks>
        public Attempt<IOfferResult<ILineItemContainer, ILineItem>> TryAwardOffer(string offerCode, bool autoAddOnSuccess = true)
        {
            // Check to make certain the customer did not already add this coupon before.  The default behavior of the 
            // line item collections will update the quantity by matching skus.
            if (ItemCache.Items.Contains(offerCode)) return Attempt<IOfferResult<ILineItemContainer, ILineItem>>.Fail(new OfferRedemptionException("This offer has already been added."));

            var foundOffer = _couponManager.Value.GetByOfferCode(offerCode, Customer);
            if (!foundOffer.Success) return Attempt<IOfferResult<ILineItemContainer, ILineItem>>.Fail(foundOffer.Exception);

            var coupon = foundOffer.Result;


            var attempt = coupon.TryApply(this.CloneItemCache(), Customer);

            if (!attempt.Success) return attempt;

            if (!autoAddOnSuccess) return attempt;

            // stuff the coupon configuration into extended data
            // this will be used to audit the redeemed offer in the Finalizing handler
            attempt.Result.Award.ExtendedData
                .SetValue(
                    Core.Constants.ExtendedDataKeys.CouponReward, 
                    JsonConvert.SerializeObject(coupon.Settings.ToOfferSettingsDisplay()));

            // save the coupon into the original item cache
            ItemCache.AddItem(attempt.Result.Award.AsLineItemOf<ItemCacheLineItem>());

            MerchelloContext.Services.ItemCacheService.Save(ItemCache);               
            return attempt;
        }


        /// <summary>
        /// The get basket checkout preparation.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The <see cref="BasketSalePreparation"/>.
        /// </returns>
        internal static BasketSalePreparation GetBasketCheckoutPreparation(IBasket basket)
        {
            return GetBasketCheckoutPreparation(Core.MerchelloContext.Current, basket);
        }

        /// <summary>
        /// The get basket checkout preparation.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The <see cref="BasketSalePreparation"/>.
        /// </returns>
        internal static BasketSalePreparation GetBasketCheckoutPreparation(IMerchelloContext merchelloContext, IBasket basket)
        {
            var customer = basket.Customer;
            var itemCache = GetItemCache(merchelloContext, customer, basket.VersionKey);

            if (!itemCache.Items.Any())
            {
                // this is either a new preparation or a reset due to version
                foreach (var item in basket.Items)
                {
                    // convert to a LineItem of the same type for use in the CheckoutPrepartion collection
                    itemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
                }
            }

            return new BasketSalePreparation(merchelloContext, itemCache, customer);
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

            return coupon.TryApply<TConstraint, TAward>(validateAgainst, Customer);
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
            var cacheKey = string.Format("merchello.basksalepreparation.offercode.{0}", offerCode);
            return (Attempt<Coupon>)_requestCache.GetCacheItem(cacheKey, () => _couponManager.Value.GetByOfferCode(offerCode, Customer));
        } 
    }
}