namespace Merchello.Web.Discounts.Coupons
{
    using System;

    using Merchello.Core.Models;

    /// <summary>
    /// The redeem coupon event args.
    /// </summary>
    public class RedeemCouponEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedeemCouponEventArgs"/> class.
        /// </summary>
        /// <param name="container">
        /// The line item container (usually the <see cref="IInvoice"/>).
        /// </param>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public RedeemCouponEventArgs(ILineItemContainer container, ILineItem lineItem)
        {
            LineItem = lineItem;
            Container = container;
        }

        /// <summary>
        /// Gets or sets the line item.
        /// </summary>
        public ILineItem LineItem { get; set; }

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        public ILineItemContainer Container { get; set; }
    }
}