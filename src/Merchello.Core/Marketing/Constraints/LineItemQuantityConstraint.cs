namespace Merchello.Core.Marketing.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this offer to line item quantity related rules.
    /// </summary>
    [OfferComponent("C679A9F7-ED13-4166-90D1-8126E314E07B", "Restrict by quantity", "This discount is only offered for line items that match configured quantity rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.lineitemquantity.html")]
    public class LineItemQuantityConstraint : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemQuantityConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public LineItemQuantityConstraint(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        public override Attempt<ILineItemContainer> Apply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}