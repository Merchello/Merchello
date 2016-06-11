namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// The product copy save.
    /// </summary>
    public class ProductCopySave
    {
        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public ProductDisplay Product { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        public string Sku { get; set; }
    }
}