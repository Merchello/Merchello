namespace Merchello.Web.Workflow
{
    using System;
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
        public Attempt<IOfferResult<ILineItemContainer, ILineItem>> TryAwardOffer(string offerCode, bool autoAddOnSuccess = true)
        {
            // Check to make certain the customer did not already add this coupon before.  The default behavior of the 
            // line item collections will update the quantity by matching skus.
            if (ItemCache.Items.Contains(offerCode)) return Attempt<IOfferResult<ILineItemContainer, ILineItem>>.Fail(new OfferRedemptionException("This offer has already been added."));

            var foundOffer = _couponManager.Value.GetByOfferCode(offerCode, Customer);
            if (!foundOffer.Success) return Attempt<IOfferResult<ILineItemContainer, ILineItem>>.Fail(foundOffer.Exception);

            var coupon = foundOffer.Result;

            // The ItemCache needs to be cloned as line items may be altered while applying constraints
            var newItemCache = new ItemCache(Guid.NewGuid(), ItemCacheType.Backoffice);
            foreach (var item in ItemCache.Items)
            {
                newItemCache.Items.Add(item.AsLineItemOf<ItemCacheLineItem>());
            }

            var attempt = coupon.TryApply(newItemCache, Customer);

            if (!attempt.Success) return attempt;

            if (!autoAddOnSuccess) return attempt;

            // stuff the coupon configuration into extended data
            // this will be used to audit the redeemed offer in the Finalizing handler
            attempt.Result.Award.ExtendedData
                .SetValue(
                    Core.Constants.ExtendedDataKeys.OfferReward, 
                    JsonConvert.SerializeObject(coupon.Settings.ToOfferSettingsDisplay()));

            // save the coupon into the original item cache
            ItemCache.AddItem(attempt.Result.Award.AsLineItemOf<ItemCacheLineItem>());

            MerchelloContext.Services.ItemCacheService.Save(ItemCache);               
            return attempt;
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
        public Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class
            where TAward : class
        {
            var foundOffer = _couponManager.Value.GetByOfferCode(offerCode, Customer);
            if (!foundOffer.Success) return Attempt<IOfferResult<TConstraint, TAward>>.Fail(foundOffer.Exception);

            var coupon = foundOffer.Result;

            return coupon.TryApply<TConstraint, TAward>(validateAgainst, Customer);
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
    }
}