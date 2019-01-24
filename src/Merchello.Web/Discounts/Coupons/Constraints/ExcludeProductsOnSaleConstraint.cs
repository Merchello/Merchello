namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System.Linq;
    
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this discount to full-priced products.
    /// </summary>
    [OfferComponent("E976981C-E256-4300-A2C2-5EDABE2A32F7", "Exclude sale products", "This discount only applies to full-price products.")]
    public class ExcludeProductsOnSaleConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeProductsOnSaleConstraintVisitor"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ExcludeProductsOnSaleConstraint(OfferComponentDefinition settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Returns whether the constraint needs configurating
        /// </summary>
        public override bool RequiresConfiguration
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public override string DisplayConfigurationFormat
        {
            get
            {
                return base.DisplayConfigurationFormat;
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
            var visitor = new ExcludeProductsOnSaleConstraintVisitor();
            value.Items.Accept(visitor);

            return visitor.FilteredItems.Any()
                       ? this.Success(this.CreateNewLineContainer(visitor.FilteredItems))
                       : this.Fail(value, "No products matched the configured filter.");
        }
    }
}