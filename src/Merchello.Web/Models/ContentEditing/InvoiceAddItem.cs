namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    ///     Item for adding a product or variant
    /// </summary>
    public class InvoiceAddItem
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public InvoiceAddItem()
        {
            Quantity = 1;
        }

        /// <summary>
        ///     The product or product variant key
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        ///     Whether or not it's a product variant key
        /// </summary>
        public bool IsProductVariant { get; set; }

        /// <summary>
        ///     The quantity to add
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     The original qty if there is one
        /// </summary>
        public int OriginalQuantity { get; set; }

        /// <summary>
        ///     Optional SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        ///  The product 
        /// </summary>
        public ProductDisplay Product { get; set; }

        /// <summary>
        ///  The product 
        /// </summary>
        public ProductVariantDisplay ProductVariant { get; set; }
    }
}