namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Caching;
    using Merchello.Web.Models;

    /// <summary>
    /// Represents a tree node value for product filter group where the keys establish the context of the filter.
    /// e.g. If x,y,z filters are already applied or the filters are to return a subset of a collection.
    /// </summary>
    internal class ProductFilterGroupNode : IMultiKeyCacheItem<IPrimedProductFilterGroup>
    {
        /// <summary>
        /// Gets or sets the collection and filter keys.
        /// </summary>
        public IEnumerable<Guid> Keys { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPrimedProductFilterGroup"/>.
        /// </summary>
        public IPrimedProductFilterGroup Item { get; set; }
    }
}