namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// A coupon reward for free shipping.
    /// </summary>
    [OfferComponent("A9375BF1-4125-4B0C-ACB9-BAD4E1BA17BE", "FREE shipping", "Applies a discount equal to the shipping charges.", RestrictToType = typeof(Coupon))]
    public class CouponFreeShippingReward : CouponDiscountLineItemRewardBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponFreeShippingReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public CouponFreeShippingReward(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public override string DisplayConfigurationFormat
        {
            get
            {
                return "'FREE shipping'";
            }
        }


        /// <summary>
        /// Gets a value indicating that this component does not require configuration.
        /// </summary>      
        public override bool RequiresConfiguration
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to apply the discount line item reward
        /// </summary>
        /// <param name="validate">
        /// The <see cref="ILineItemContainer"/> to validate against
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{ILinetItem}"/>.
        /// </returns>
        public override Attempt<ILineItem> TryAward(ILineItemContainer validate, ICustomerBase customer)
        {
            var shippingLineItems = validate.ShippingLineItems();
            var audits = shippingLineItems.Select(item => 
                new CouponRewardAdjustmentAudit()
                    {
                        RelatesToSku = item.Sku, 
                        Log = new[]
                                  {
                                      new DataModifierLog()
                                          {
                                              PropertyName = "Price", 
                                              OriginalValue = item.Price, 
                                              ModifiedValue = 0M
                                          }
                                  }
                    }).ToList();

            // Get the item template
            var discountLineItem = CreateTemplateDiscountLineItem(audits);
            var discount = validate.ShippingLineItems().Sum(x => x.TotalPrice);            
            discountLineItem.Price = discount;
           
            return Attempt<ILineItem>.Succeed(discountLineItem);
        }
    }
}