namespace Merchello.Web.Workflow.InvoiceCreation
{
    using System;
    using System.Linq;

    using Merchello.Core.Chains.InvoiceCreation;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Web.Discounts.Coupons;

    using Umbraco.Core;

    /// <summary>
    /// Adds any coupon discounts to the invoice.
    /// </summary>
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
                return _couponManager.Value;
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
            if (!SalePreparation.OfferCodes.Any()) return Attempt<IInvoice>.Succeed(value);

            if (!(SalePreparation is IBasketSalePreparation)) 
                return Attempt<IInvoice>.Fail(value, new InvalidCastException("SalePreparation object is not IBasketSalePreparation"));
            _basketSalePreparation = SalePreparation as IBasketSalePreparation;

            foreach (var code in SalePreparation.OfferCodes)
            {
                var foundCoupon = CouponOfferManager.GetByOfferCode(code, SalePreparation.Customer);
                if (!foundCoupon.Success)
                {
                    continue;
                }
                
                var coupon = foundCoupon.Result;
                var clone = LineItemExtensions.CreateNewBackOfficeLineItemContainer(value.Items);
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