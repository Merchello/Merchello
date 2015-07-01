namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this discount to a selection of products.
    /// </summary>
    [OfferComponent("15DDF0EA-9C60-489A-96A8-D2AAADBEF328", "Product rules", "This discount is only offered for certain products.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.productselectionfilter.html")]
    public class ProductSelectionFilterConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSelectionFilterConstraint"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ProductSelectionFilterConstraint(OfferComponentDefinition settings)
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

        /// <summary>
        /// Gets the product constraint data set.
        /// </summary>
        private IEnumerable<ProductConstraintData> ProductConstraintDataSet
        {
            get
            {
                var config = this.GetConfigurationValue("productConstraints");
                if (string.IsNullOrEmpty(config)) return Enumerable.Empty<ProductConstraintData>();

                return JsonConvert.DeserializeObject<IEnumerable<ProductConstraintData>>(config);
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
            var visitor = new ProductSelectionConstraintVisitor(this.ProductConstraintDataSet);
            value.Items.Accept(visitor);

            return visitor.FilteredItems.Any()
                       ? this.Success(this.CreateNewLineContainer(visitor.FilteredItems))
                       : this.Fail(value, "No products matched the configured filter.");
        }
    }
}