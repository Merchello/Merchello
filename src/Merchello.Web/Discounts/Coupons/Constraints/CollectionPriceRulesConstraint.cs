namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;
    using System.Linq;

    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Price constraint rules for the entire qualifying collection
    /// </summary>
    [OfferComponent("01CABB7B-F718-4639-A260-EB22E950DBE6", "Total price of items rules", "Tests the total price all line items against configured rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.collectionpricerules.html", typeof(Coupon))]
    public class CollectionPriceRulesConstraint : CouponConstraintBase
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

                return string.Format("'Total price is {0} ' +  $filter('currency')({1}, $scope.currencySymbol)", operatorText, price);
            }
        }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        private decimal Price
        {
            get
            {
                decimal price;
                return decimal.TryParse(this.GetConfigurationValue("price"), out price) ? price : 0;
            }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        private string Operator
        {
            get
            {
                var value = this.GetConfigurationValue("operator");
                return string.IsNullOrEmpty(value) ? "gt" : value;
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
            if (Price <= 0) return Attempt<ILineItemContainer>.Succeed(value);

            var total = value.Items.Sum(x => x.TotalPrice);

            return StringOperatorHelper.Evaluate(total, Price, Operator) ? 
                this.Success(value) : 
                this.Fail(value, "The total price of items failed to pass the configured condition.");
        }
    }
}