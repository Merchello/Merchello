namespace Merchello.Core.Marketing.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this offer to line item quantity related rules.
    /// </summary>
    [OfferComponent("C679A9F7-ED13-4166-90D1-8126E314E07B", "Restrict by product quantity", "This discount is only offered for product line items that match configured quantity rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.productlineitemquantity.html")]
    public class ProductLineItemQuantityConstraint : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductLineItemQuantityConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ProductLineItemQuantityConstraint(OfferComponentDefinition definition)
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

        public override Attempt<ILineItemContainer> Apply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }


    }
}