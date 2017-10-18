namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// Item for adding a product or variant
    /// </summary>
    public class InvoiceAddItem
    {
        /// <summary>
        ///     The product or product variant key
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        ///     Whether or not it's a product variant key
        /// </summary>
        public bool IsProductVariant { get; set; }
    }
}