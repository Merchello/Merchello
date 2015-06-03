namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a discount line item reward.
    /// </summary>
    [OfferComponent("A1CCE36A-C5AA-4C50-B659-CC2FBDEAA7B3", "Discount the price", "Applies a discount according to configured price rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.couponreward.discountprice.html", typeof(Coupon))]
    public class CouponDiscountLineItemReward : OfferRewardComponentBase<ILineItemContainer, ILineItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponDiscountLineItemReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public CouponDiscountLineItemReward(OfferComponentDefinition definition)
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
                var amount = this.GetConfigurationValue("amount");
                var adjustmentType = this.GetConfigurationValue("adjustmentType");

                if (string.IsNullOrEmpty("amount") || string.IsNullOrEmpty("adjustmentType")) return "''";

                return
                    string.Format(
                        adjustmentType == "percent"
                            ? "'Discount price: {0}%'"
                            : "'Discount price: ' + $filter('currency')({0}, $scope.currencySymbol)",
                        amount);
            }
        }

        public override Attempt<ILineItem> Award(ILineItemContainer validate, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}