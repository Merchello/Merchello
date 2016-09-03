namespace Merchello.Web.Models
{
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.Ui.Rendering;

    /// <summary>
    /// The product collection.
    /// </summary>
    internal class ProductCollection : EntityCollectionProxyBase, IProductCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollection"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ProductCollection(IEntityCollection collection) 
            : base(collection)
        {
        }
    }
}