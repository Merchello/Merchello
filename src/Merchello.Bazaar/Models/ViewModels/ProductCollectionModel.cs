namespace Merchello.Bazaar.Models.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The product collection model.
    /// </summary>
    public class ProductCollectionModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public ProductCollectionModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        public IEnumerable<IProductContent> Products
        {
            get
            {
                return Content.HasValue("products")
                           ? Content.GetPropertyValue<IEnumerable<IProductContent>>("products")
                           : Enumerable.Empty<IProductContent>();
            }
        }
    }
}