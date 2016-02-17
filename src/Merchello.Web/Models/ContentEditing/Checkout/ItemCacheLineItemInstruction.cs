namespace Merchello.Web.Models.ContentEditing.Checkout
{
    using System;

    /// <summary>
    /// The item cache instruction.
    /// </summary>
    public class ItemCacheInstruction : ItemCacheInstructionBase
    {
        /// <summary>
        /// Gets or sets the instruction entity key.
        /// </summary>
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }
    }
}
