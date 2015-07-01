namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this offer to line item quantity related rules.
    /// </summary>
    [OfferComponent("C679A9F7-ED13-4166-90D1-8126E314E07B", "Line item quantity rules", "Filters the line item collection for individual product line items quantities matching configured rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.filterquantityrules.html", typeof(Coupon))]
    public class LineItemQuantityFilterRulesConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemQuantityFilterRulesConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public LineItemQuantityFilterRulesConstraint(OfferComponentDefinition definition)
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
                var quantity = this.GetConfigurationValue("quantity");
                var op = this.GetConfigurationValue("operator");

                var operatorText = StringOperatorHelper.TextForOperatorString(op);

                if (string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(operatorText)) return "''";

                return string.Format("'Quantity is {0} {1}'", operatorText, quantity);
            }
        }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        private decimal Quantity
        {
            get
            {
                int quantity;
                return int.TryParse(this.GetConfigurationValue("quantity"), out quantity) ? quantity : 0;
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
            var visitor = new NumericalValueFilterConstraintVisitor(Quantity, Operator, "quantity");
            value.Items.Accept(visitor);

            return visitor.FilteredLineItems.Any(x => x.LineItemType == LineItemType.Product)
                       ? this.Success(CreateNewLineContainer(visitor.FilteredLineItems))
                       : this.Fail(value, "No items qualify. Resulting collection would be empty.");
        }
    }
}