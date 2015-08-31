namespace Merchello.Web.Validation.Tasks
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core;

    /// <summary>
    /// Validates that products still exist in Merchello.
    /// </summary>
    internal class ValidateProductsExistTask : CustomerItemCacheValidatationTaskBase<ValidationResult<CustomerItemCacheBase>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateProductsExistTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers in the MerchelloHelper
        /// </param>
        public ValidateProductsExistTask(IMerchelloContext merchelloContext, bool enableDataModifiers)
            : base(merchelloContext, enableDataModifiers)
        {
        }

        /// <summary>
        /// The perform task.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{IItemCache}"/>.
        /// </returns>
        public override Attempt<ValidationResult<CustomerItemCacheBase>> PerformTask(ValidationResult<CustomerItemCacheBase> value)
        {
            var visitor = new ProductSkuExistsValidationVisitor(Merchello);
            value.Validated.Accept(visitor);

            if (visitor.LineItemsToRemove.Any())
            {
                foreach (var item in visitor.LineItemsToRemove)
                {
                    value.Validated.RemoveItem(item.Sku);
                    value.Messages.Add("Item no longer exists.  Item removed was " + item.Sku);
                    value.Validated.Save();
                }
            }

            return Attempt<ValidationResult<CustomerItemCacheBase>>.Succeed(value);
        }
    }
}