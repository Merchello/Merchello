namespace Merchello.Web.Validation.Tasks
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core;

    /// <summary>
    /// Validates the product is in inventory.
    /// </summary>
    internal class ValidateProductInventoryTask : CustomerItemCacheValidatationTaskBase<ValidationResult<CustomerItemCacheBase>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateProductInventoryTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        public ValidateProductInventoryTask(IMerchelloContext merchelloContext, bool enableDataModifiers)
            : base(merchelloContext, enableDataModifiers)
        {
        }

        /// <summary>
        /// Performs the task of checking the inventory.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<ValidationResult<CustomerItemCacheBase>> PerformTask(ValidationResult<CustomerItemCacheBase> value)
        {
            var visitor = new ProductInventoryValidationVisitor(Merchello);

            value.Validated.Accept(visitor);

            if (!visitor.OutOfStockItems.Any()) return Attempt<ValidationResult<CustomerItemCacheBase>>.Succeed(value);

            foreach (var item in visitor.OutOfStockItems.ToArray())
            {
                value.Validated.RemoveItem(item.Sku);
                value.Messages.Add("Item is out of stock.  Item removed was " + item.Sku);
                value.Validated.Save();
            }

            return Attempt<ValidationResult<CustomerItemCacheBase>>.Succeed(value);
        }
    }
}