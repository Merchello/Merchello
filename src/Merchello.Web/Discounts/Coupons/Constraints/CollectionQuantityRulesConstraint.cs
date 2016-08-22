namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;
    using System.Linq;

    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this offer to line item quantity related rules.
    /// </summary>
    [OfferComponent("C7F0B590-11E7-4986-A52E-F18D0382E07E", "Total quantity of items rules", "Tests the total quantity all line items against configured rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.collectionquantityrules.html", typeof(Coupon))]
    public class CollectionQuantityRulesConstraint : CouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionQuantityRulesConstraint"/> class. 
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public CollectionQuantityRulesConstraint(OfferComponentDefinition definition)
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

                return string.Format("'Total quantity is {0} {1}'", operatorText, quantity);
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
            if (Quantity <= 0) return Attempt<ILineItemContainer>.Succeed(value);

            var total = value.Items.Sum(x => x.Quantity);

            return StringOperatorHelper.Evaluate(total, Quantity, Operator) ?
                this.Success(value) :
                this.Fail(value, "The total quantity of items failed to pass the configured condition.");
        }
    }
}