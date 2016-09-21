namespace Merchello.Web.Models.Usage
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The product filter group results count.
    /// </summary>
    internal class ProductFilterGroupResultsCount : IProductFilterGroupResultsCount
    {

        public Guid Key { get; }

        public Guid? ParentKey { get; }

        public string Name { get; }

        public int SortOrder { get; }

        public IProviderMeta ProviderMeta { get; }

        public IEnumerable<Guid> CollectionKeys { get; }

        public int Count { get; }

        public IEnumerator<IProductFilterResultsCount> Filters { get; }
    }
}