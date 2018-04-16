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
        public bool IsProductVariant => Product == null;

        /// <summary>
        /// Whether or not this is a custom product so allows editing
        /// </summary>
        public bool NeedsUpdating
        {
            get
            {
                // Check both are null
                if (Product == null && ProductVariant == null)
                {
                    // Now check data
                    if (Quantity != OriginalQuantity ||
                        Sku != OriginalSku ||
                        Price != OriginalPrice || 
                        Name != OriginalName)
                    {
                        // If we are here, something has changed
                        return true;
                    }
                }

                return false;
            }            
        }

        /// <summary>
        ///     The quantity to add
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     The original qty if there is one
        /// </summary>
        public int OriginalQuantity { get; set; }

        /// <summary>
        ///     The submitted SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        ///     The submitted SKU
        /// </summary>
        public string OriginalSku { get; set; }

        /// <summary>
        /// Submitted Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Original Price
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        ///     The submitted Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The submitted Name
        /// </summary>
        public string OriginalName { get; set; }

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