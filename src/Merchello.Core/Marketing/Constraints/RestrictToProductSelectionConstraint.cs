namespace Merchello.Core.Marketing.Constraints
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this discount to a selection of products.
    /// </summary>
    [OfferComponent("15DDF0EA-9C60-489A-96A8-D2AAADBEF328", "Restrict to certain products", "This discount is only offered for certain products.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.restricttoproductselection.html")]
    public class RestrictToProductSelectionConstraint : OfferConstraintComponentBase<ILineItemContainer>
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

        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public override string DisplayConfigurationFormat
        {
            get
            {
                if (MerchelloContext.Current == null)
                {
                    return base.DisplayConfigurationFormat;
                }

                if (this.OfferComponentDefinition.ExtendedData.IsEmpty) return "''";

                var config = this.GetConfigurationValue("productConstraints");

                var constraints = JsonConvert.DeserializeObject<IEnumerable<ProductConstraintData>>(config);

                var displayText = constraints.GetUiDisplayText();

                return string.Format("'{0}'", displayText);
            }
        }

        public override Attempt<ILineItemContainer> TryApply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new NotImplementedException();
        }
    }
}