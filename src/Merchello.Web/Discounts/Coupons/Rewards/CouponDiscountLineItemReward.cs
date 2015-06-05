namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using System.Globalization;

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
        /// The adjustment.
        /// </summary>
        private enum Adjustment
        {
            Flat,
            Percent,
            NotSet
        }

        #region properties

        /// <summary>
        /// Gets a value indicating whether is configured.
        /// </summary>
        private bool IsConfigured
        {
            get
            {
                return AdjustmentType != Adjustment.NotSet;
            }    
        }

        /// <summary>
        /// Gets the configured amount.
        /// </summary>
        private decimal Amount
        {
            get
            {
                decimal converted;
                return decimal.TryParse(this.GetConfigurationValue("amount"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out converted) ? converted : 0;                                
            }
        }

        /// <summary>
        /// Gets the adjustment type.
        /// </summary>
        private Adjustment AdjustmentType
        {
            get
            {
                var adjustmentType = this.GetConfigurationValue("adjustmentType");   
                if (string.IsNullOrEmpty(adjustmentType)) return Adjustment.NotSet;

                return Enum<Adjustment>.Parse(adjustmentType, true);
            }
        }

        /// <summary>
        /// Gets the max quantity.
        /// </summary>
        private int MaxQuantity
        {
            get
            {
                int converted;
                return int.TryParse(this.GetConfigurationValue("maxQuantity"), out converted) ? converted : 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether apply to discount to each matching item
        /// </summary>
        private bool ApplyToEachMatching
        {
            get
            {
                return string.Equals("True", this.GetConfigurationValue("applyToEachMatching"));
            }
        }

        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public override string DisplayConfigurationFormat
        {
            get
            {

                if (!IsConfigured) return "''";

                return
                    string.Format(
                        AdjustmentType == Adjustment.Percent
                            ? "'Discount price: {0}%'"
                            : "'Discount price: ' + $filter('currency')({0}, $scope.currencySymbol)",
                        Amount);
            }
        }

        #endregion

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
            throw new System.NotImplementedException();
        }
    }
}