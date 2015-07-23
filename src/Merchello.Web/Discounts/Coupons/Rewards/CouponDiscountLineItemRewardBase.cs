namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Newtonsoft.Json;

    /// <summary>
    /// The coupon discount line item reward base.
    /// </summary>
    public abstract class CouponDiscountLineItemRewardBase : OfferRewardComponentBase<ILineItemContainer, ILineItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponDiscountLineItemRewardBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        protected CouponDiscountLineItemRewardBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Creates a template discount line item.
        /// </summary>
        /// <param name="audits">
        /// Reward audit log
        /// </param>
        /// <returns>
        /// The <see cref="ILineItem"/>.
        /// </returns>
        protected ILineItem CreateTemplateDiscountLineItem(IEnumerable<CouponRewardAdjustmentAudit> audits = null)
        {
            // get the discount line item setup
            // assume being executed from sale preparation so it will be an ItemCacheLineItem
            var discountLineItem = new ItemCacheLineItem(
                LineItemType.Discount,
                this.GetRewardLineItemName(),
                this.OfferCode,
                1,
                0);

            if (audits != null)
            {
                discountLineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.CouponRewardLog, JsonConvert.SerializeObject(audits));
            }

            return discountLineItem;
        }
    }
}