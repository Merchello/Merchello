namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a product filter.
    /// </summary>
    internal class ProductFilter : IProductFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilter"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ProductFilter(IEntityCollection collection)
        {
            this.Key = collection.Key;
            this.ProviderKey = collection.ProviderKey;
            this.SortOrder = collection.SortOrder;
            this.Name = collection.Name;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key { get; private set; }

        /// <summary>
        /// Gets the provider key.
        /// </summary>
        public Guid ProviderKey { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        internal int SortOrder { get; private set; }
    }
}