namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The product filter group.
    /// </summary>
    internal class ProductFilterGroup : IProductFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroup"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ProductFilterGroup(IEntitySpecifiedFilterCollection collection)
        {
            this.Key = collection.Key;
            this.Name = collection.Name;
            this.SortOrder = collection.SortOrder;
            this.Initialize(collection.AttributeCollections);
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public IEnumerable<IProductFilter> Filters { get; private set; }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="attributes">
        /// The actual filter collections.
        /// </param>
        private void Initialize(IEnumerable<IEntityCollection> attributes)
        {
            this.Filters = attributes.Select(x => new ProductFilter(x)).OrderBy(x => x.SortOrder);
        }
    }
}