namespace Merchello.Bazaar.Models.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The product collection model.
    /// </summary>
    public partial class ProductCollectionModel : MasterModel
    {
        /// <summary>
        /// The _products.
        /// </summary>
        private IProductContent[] _products;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollectionModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public ProductCollectionModel(IPublishedContent content)
            : base(content)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        public IEnumerable<IProductContent> Products
        {
            get
            {
                return _products;
            }
        }

        /// <summary>
        /// The specify culture.
        /// </summary>
        /// <param name="culture">
        /// The culture.
        /// </param>
        public void SpecifyCulture(CultureInfo culture)
        {
            _products.ForEach(x => x.SpecifyCulture(culture.Name));
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        private void Initialize()
        {
            _products = Content.HasValue("products")
                           ? Content.GetPropertyValue<IEnumerable<IProductContent>>("products").ToArray()
                           : Enumerable.Empty<IProductContent>().ToArray();
        }
    }
}