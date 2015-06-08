namespace Merchello.Web.Workflow.InvoiceCreation
{
    using System;
    using System.Linq;

    using Merchello.Core.Chains.InvoiceCreation;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// Adds any coupon discounts to the invoice.
    /// </summary>
    internal class AddCouponDiscountsToInvoiceTask : InvoiceCreationAttemptChainTaskBase
    {
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

            throw new NotImplementedException();
        }
    }
}