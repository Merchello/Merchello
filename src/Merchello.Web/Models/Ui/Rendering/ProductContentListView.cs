namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// The value returned for the Merchello Product List View data type.
    /// </summary>
    public class ProductContentListView : IEnumerable<IProductContent>
    {
        /// <summary>
        /// The collection of <see cref="IProductContent"/>.
        /// </summary>
        private readonly IEnumerable<IProductContent> _products;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentListView"/> class.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="data">
        /// The collection of products.
        /// </param>
        public ProductContentListView(Guid collectionKey, IEnumerable<IProductContent> data)
        {
            this.CollectionKey = collectionKey;
            this._products = data ?? Enumerable.Empty<IProductContent>();
        }

        /// <summary>
        /// Gets or sets the collection key.
        /// </summary>
        public Guid CollectionKey { get; set; }

        /// <summary>
        /// Gets the <see cref="IEnumerator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<IProductContent> GetEnumerator()
        {
            return this._products.GetEnumerator();
        }

        /// <summary>
        /// The the <see cref="IEnumerator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}