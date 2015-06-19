namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The maximum quantity constraint.
    /// </summary>
    [OfferComponent("AC69429B-3941-4EF1-9B86-90D576848D99", "Maximum qualifying quantity", "Limits to discount to a maximum quantity per line. (eg. Customer orders 5 units but this discount only should only be applied to 2)",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.maximumquantity.html", typeof(Coupon))]
    public class MaximumQuantityConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximumQuantityConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public MaximumQuantityConstraint(OfferComponentDefinition definition)
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
                var maximum = this.GetConfigurationValue("maximum");
                
                // price and operator
                if (string.IsNullOrEmpty(maximum)) return string.Empty;

                return string.Format("'Maximum qualifying quantity: {0} '", maximum);
            }
        }

        /// <summary>
        /// Gets the maximum quantity.
        /// </summary>
        private int MaximumQuantity
        {
            get
            {
                int max;
                return int.TryParse(this.GetConfigurationValue("maximum"), out max) ? max : 0;
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
            var visitor = new MaximumQuantityConstraintVisitor(MaximumQuantity);
            value.Items.Accept(visitor);

            return this.Success(this.CreateNewLineContainer(visitor.ModifiedItems));
        }
    }
}