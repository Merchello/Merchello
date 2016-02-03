namespace Merchello.Web.Models.ContentEditing.Checkout
{
    /// <summary>
    /// The item cache line item instruction.
    /// </summary>
    public class ItemCacheLineItemInstruction : ItemCacheInstructionBase
    {
        /// <summary>
        /// Gets or sets the <see cref="ProductVariantDisplay"/>.
        /// </summary>
        public ItemCacheLineItemDisplay LineItem { get; set; }
    }
}
