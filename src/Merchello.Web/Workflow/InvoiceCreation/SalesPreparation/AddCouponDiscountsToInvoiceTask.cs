namespace Merchello.Web.Workflow.InvoiceCreation.SalesPreparation
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains.InvoiceCreation.SalesPreparation;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Web.Discounts.Coupons;

    using Umbraco.Core;

    /// <summary>
    /// Adds any coupon discounts to the invoice.
    /// </summary>
    [Obsolete("Superseded by CheckoutManager.AddCouponDiscountsToInvoiceTask")]
    internal class AddCouponDiscountsToInvoiceTask : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// The basket sale preparation.
        /// </summary>
        private IBasketSalePreparation _basketSalePreparation;

        /// <summary>
        /// The <see cref="CouponManager"/>.
        /// </summary>
        private Lazy<CouponManager> _couponManager = new Lazy<CouponManager>(() => CouponManager.Instance); 

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCouponDiscountsToInvoiceTask"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The <see cref="IBasketSalePreparation"/>.
        /// </param>
        public AddCouponDiscountsToInvoiceTask(SalePreparationBase salePreparation)
            : base(salePreparation)
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
            if (!this.SalePreparation.OfferCodes.Any()) return Attempt<IInvoice>.Succeed(value);

            if (!(this.SalePreparation is IBasketSalePreparation)) 
                return Attempt<IInvoice>.Fail(value, new InvalidCastException("SalePreparation object is not IBasketSalePreparation"));
            this._basketSalePreparation = this.SalePreparation as IBasketSalePreparation;

            foreach (var code in this.SalePreparation.OfferCodes)
            {
                var foundCoupon = this.CouponOfferManager.GetByOfferCode(code, this.SalePreparation.Customer);
                if (!foundCoupon.Success)
                {
                    continue;
                }
                
                var coupon = foundCoupon.Result;
                var clone = Extensions.CreateNewItemCacheLineItemContainer(value.Items.Where(x => x.LineItemType != LineItemType.Tax));
                var apply = coupon.TryApply(clone, this.SalePreparation.Customer).AsCouponRedemptionResult(coupon);
                if (apply.Success)
                {                    
                    this.CouponOfferManager.SafeAddCouponAttemptContainer<InvoiceLineItem>(value, apply, true);  
                }
            }

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}