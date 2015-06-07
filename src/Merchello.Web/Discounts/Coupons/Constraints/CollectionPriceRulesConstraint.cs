namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Price constraint rules for the entire qualifying collection
    /// </summary>
    [OfferComponent("66957C56-8A5E-4ECD-BDEB-565F8777A38F", "Filter by product line item price rules", "Filters the line item collection for individual product line items with prices matching configured rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.filterpricerules.html", typeof(Coupon))]
    public class CollectionPriceRulesConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionPriceRulesConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public CollectionPriceRulesConstraint(OfferComponentDefinition definition)
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
                var price = this.GetConfigurationValue("price");
                var op = this.GetConfigurationValue("operator");

                var operatorText = StringOperatorHelper.TextForOperatorString(op);

                // price and operator
                if (string.IsNullOrEmpty(price) || string.IsNullOrEmpty(operatorText)) return string.Empty;

                return string.Format("'Price is {0} ' +  $filter('currency')({1}, $scope.currencySymbol)", operatorText, price);
            }
        }

        /// <summary>
        /// Validates the constraint against the <see cref="ILineItemContainer"/>
        /// </summary>
        /// <param name="value">
        /// The value to object to which the constraint is to be applied.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{ILineItemContainer}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        public override Attempt<ILineItemContainer> TryApply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}