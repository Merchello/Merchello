namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The coupon reward adjustment audit.
    /// </summary>
    public class CouponRewardAdjustmentAudit
    {
        /// <summary>
        /// Gets or sets the SKU for the line item to which this coupon adjustment relates.
        /// </summary>
        public string RelatesToSku { get; set; }

        /// <summary>
        /// Gets or sets the log of adjustments.
        /// </summary>
        public IEnumerable<IDataModifierLog> Log { get; set; }
    }
}