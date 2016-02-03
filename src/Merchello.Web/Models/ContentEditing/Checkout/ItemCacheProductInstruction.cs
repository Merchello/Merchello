namespace Merchello.Web.Models.ContentEditing.Checkout
{
    /// <summary>
    /// The customer item cache instruction.
    /// </summary>
    public class ItemCacheProductInstruction : ItemCacheInstructionBase
    {
        /// <summary>
        /// Gets or sets the <see cref="ProductVariantDisplay"/>.
        /// </summary>
        public ProductVariantDisplay ProductVariant { get; set; }
    }
}
