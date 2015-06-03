namespace Merchello.Core.Marketing.Constraints
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this discount to a selection of products.
    /// </summary>
    [OfferComponent("15DDF0EA-9C60-489A-96A8-D2AAADBEF328", "Restrict to certain products", "This discount is only offered for certain products.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.restricttoproductselection.html")]
    public sealed class RestrictToProductSelectionConstraint : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictToProductSelectionConstraint"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public RestrictToProductSelectionConstraint(OfferComponentDefinition settings)
            : base(settings)
        {
        }

        public override Attempt<ILineItemContainer> Apply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new NotImplementedException();
        }        
    }
}