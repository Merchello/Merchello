namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// A base class for proxy entity collections.
    /// </summary>
    internal abstract class EntityCollectionProxyBase : IEntityCollectionProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProxyBase"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        protected EntityCollectionProxyBase(IEntityCollection collection)
        {
            this.Key = collection.Key;
            this.ProviderKey = collection.ProviderKey;
            this.ParentKey = collection.ParentKey;
            this.SortOrder = collection.SortOrder;
            this.Name = collection.Name;
        }

        /// <summary>
        /// Gets the parent key.
        /// </summary>
        public Guid? ParentKey { get; private set; }

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
        /// Gets the provider key.
        /// </summary>
        internal Guid ProviderKey { get; private set; }
    }
}