namespace Merchello.Web.Validation.Tasks
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Validates that products still exist in Merchello.
    /// </summary>
    public class ValidateItemCacheProductsExistTask : ValidatationTaskBase<IItemCache>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateItemCacheProductsExistTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers in the MerchelloHelper
        /// </param>
        public ValidateItemCacheProductsExistTask(IMerchelloContext merchelloContext, bool enableDataModifiers)
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
        public override Attempt<IItemCache> PerformTask(IItemCache value)
        {
            var visitor = new ProductSkuExistsVisitor(Merchello);
            value.Items.Accept(visitor);

            if (visitor.LineItemsToRemove.Any())
            {
                foreach (var item in visitor.LineItemsToRemove)
                {
                    value.Items.Remove(item.Sku);
                }
            }

            return Attempt<IItemCache>.Succeed(value);
        }
    }
}