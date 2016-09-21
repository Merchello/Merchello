namespace Merchello.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The product filter group.
    /// </summary>
    public class ProductFilterGroup : IProductFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroup"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ProductFilterGroup(IEntityFilterGroup collection)
        {
            this.Key = collection.Key;
            this.Name = collection.Name;
            this.SortOrder = collection.SortOrder;
            this.Initialize(collection.Filters, collection.ProviderKey);
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
        /// Gets information about the managing provider.
        /// </summary>
        public IProviderMeta ProviderMeta { get; private set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public IEnumerable<IProductFilter> Filters { get; private set; }

        /// <summary>
        /// Gets the parent key.
        /// </summary>
        public Guid? ParentKey
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="filters">
        /// The actual filter collections.
        /// </param>
        /// <param name="providerKey">The key for the provider responsible for managing this filter</param>
        private void Initialize(IEnumerable<IEntityCollection> filters, Guid providerKey)
        {
            this.Filters = filters
                .Where(x => x.EntityTfKey == Core.Constants.TypeFieldKeys.Entity.ProductKey && x.IsFilter)
                .Select(x => new ProductFilter(x)).OrderBy(x => x.SortOrder);

            this.ProviderMeta = new ProviderMeta(providerKey);
        }
    }
}