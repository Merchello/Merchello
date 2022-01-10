namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using System.Globalization;
    using System.Linq;
    using Umbraco.Core;

    /// <summary>
    /// Represents a discount line item reward.
    /// </summary>
    [OfferComponent("A1CCE36A-C5AA-4C50-B659-CC2FBDEAA7B3", "Discount the price", "Applies a discount to items that pass constraint filters according to configured price rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.couponreward.discountprice.html", typeof(Coupon))]
    public class CouponDiscountLineItemReward : CouponDiscountLineItemRewardBase
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
        internal enum Adjustment
        {
            Flat,
            Percent,
            NotSet
        }

        #region properties

        /// <summary>
        /// Gets the adjustment type.
        /// </summary>
        internal Adjustment AdjustmentType
        {
            get
            {
                var adjustmentType = this.GetConfigurationValue("adjustmentType");
                if (string.IsNullOrEmpty(adjustmentType)) return Adjustment.NotSet;

                return Enum<Adjustment>.Parse(adjustmentType, true);
            }
        }


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
            if (!IsConfigured) return Attempt<ILineItem>.Fail(new OfferRedemptionException("The coupon reward is not configured."));
            if (MerchelloContext.Current == null) return Attempt<ILineItem>.Fail(new OfferRedemptionException("The MerchelloContext was null"));

            // apply to the entire collection excluding previously added discounts
            var qualifying =
                Extensions.CreateNewItemCacheLineItemContainer(validate.Items.Where(x => x.LineItemType != LineItemType.Discount));

            var visitor = new CouponDiscountLineItemRewardVisitor(Amount, AdjustmentType);
            qualifying.Items.Accept(visitor);

            var qualifyingTotal = visitor.QualifyingTotal;


            var discount = this.AdjustmentType == Adjustment.Flat
                                    ? this.Amount > qualifyingTotal ? qualifyingTotal : this.Amount
                                    : qualifyingTotal * (this.Amount / 100);

            // Get the item template
            var discountLineItem = CreateTemplateDiscountLineItem(visitor.Audits);
            discountLineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.CouponAdjustedProductPreTaxTotal, visitor.AdjustedProductPreTaxTotal.ToString(CultureInfo.InvariantCulture));
            discountLineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.CouponAdjustedProductTaxTotal, visitor.AdjustedTaxTotal.ToString(CultureInfo.InvariantCulture));
            discountLineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, true.ToString());
            discountLineItem.Price = discount;

            return Attempt<ILineItem>.Succeed(discountLineItem);
        }
    }
}