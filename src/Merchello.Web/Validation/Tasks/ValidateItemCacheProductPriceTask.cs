namespace Merchello.Web.Validation.Tasks
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Validates that product pricing in Merchello has not been changed.
    /// </summary>
    /// <remarks>
    /// Note: for multi-currency sites, you should place the extended data key (merchLineItemAllowsValidation = false) 
    /// into the line item extended data so that this task
    /// does not reset the price to that set in the back office
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class ValidateItemCacheProductPriceTask : ValidatationTaskBase<IItemCache>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateItemCacheProductPriceTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModfiers">
        /// The enable data modfiers.
        /// </param>
        public ValidateItemCacheProductPriceTask(IMerchelloContext merchelloContext, bool enableDataModfiers)
            : base(merchelloContext, enableDataModfiers)
        {
        }

        /// <summary>
        /// Performs the task of validating the pricing information.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IItemCache> PerformTask(IItemCache value)
        {
            var visitor = new ProductPricingVisitor(Merchello);

            value.Items.Accept(visitor);

            if (visitor.InvalidPrices.Any())
            {
                LogHelper.Info<ValidateItemCacheProductPriceTask>("Invalid pricing found");
            }

            return Attempt<IItemCache>.Succeed(value);
        }
    }
}