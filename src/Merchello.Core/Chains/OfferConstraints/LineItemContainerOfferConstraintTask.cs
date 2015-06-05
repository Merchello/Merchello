namespace Merchello.Core.Chains.OfferConstraints
{
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A constraint task for constraints that pass <see cref="LineItemCollection"/>s
    /// </summary>
    public class LineItemContainerOfferConstraintTask : OfferConstraintChainTask<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemContainerOfferConstraintTask"/> class.
        /// </summary>
        /// <param name="component">
        /// The component.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        public LineItemContainerOfferConstraintTask(OfferConstraintComponentBase<ILineItemContainer> component, ICustomerBase customer)
            : base(component, customer)
        {
        }

        /// <summary>
        /// Executes the component task
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{LineItemCollection}"/>.
        /// </returns>
        public override Attempt<ILineItemContainer> PerformTask(ILineItemContainer value)
        {
            return Component.TryApply(value, Customer);
        }
    }
}