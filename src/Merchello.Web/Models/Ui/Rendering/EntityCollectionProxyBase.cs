namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// A base class for proxy entity collections.
    /// </summary>
    public abstract class EntityCollectionProxyBase : IEntityCollectionProxy
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
            this.ParentKey = collection.ParentKey;
            this.ProviderKey = collection.ProviderKey;
            this.SortOrder = collection.SortOrder;
            this.Name = collection.Name;
            this.ExtendedData = collection.ExtendedData;

            this.Initialize(collection.ProviderKey);
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
        /// Gets the provider meta.
        /// </summary>
        public IProviderMeta ProviderMeta { get; private set; }

        /// <summary>
        /// Gets the extended data collection.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; private set; }

        /// <summary>
        /// Gets the provider key.
        /// </summary>
        internal Guid ProviderKey { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        private void Initialize(Guid providerKey)
        {
            this.ProviderMeta = new ProviderMeta(providerKey);
        }
    }
}