namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Filters the line item collection for configured price rules."
    /// </summary>
    [OfferComponent("66957C56-8A5E-4ECD-BDEB-565F8777A38F", "Line item price rules", "Filters the line item collection for individual product line items with prices matching configured rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.filterpricerules.html", typeof(Coupon))]
    public class LineItemPriceFilterRulesConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemPriceFilterRulesConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public LineItemPriceFilterRulesConstraint(OfferComponentDefinition definition)
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
            var visitor = new NumericalValueFilterConstraintVisitor(this.Price, Operator, "price");
            value.Items.Accept(visitor);

            return visitor.FilteredLineItems.Any(x => x.LineItemType == LineItemType.Product)
                       ? this.Success(CreateNewLineContainer(visitor.FilteredLineItems))
                       : this.Fail(value, "No items qualify. Quantity filter removes all items");
        }
    }
}