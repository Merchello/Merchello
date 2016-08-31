namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;

    /// <summary>
    /// Represents meta data about a provider.
    /// </summary>
    internal class ProviderMeta : IProviderMeta
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderMeta"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        public ProviderMeta(Guid providerKey)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey cannot be an Empty GUID");
            this.Key = providerKey;
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderMeta"/> class.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        internal ProviderMeta(EntityCollectionProviderAttribute attribute)
        {
            Ensure.ParameterNotNull(attribute, "EntityCollectionProviderAttribute cannot be null");
            this.Key = attribute.Key;
            this.Name = attribute.Name;
            this.Description = attribute.Description;
            this.ManagesUniqueCollection = attribute.ManagesUniqueCollection;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key { get; private set;  }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether manages unique collection.
        /// </summary>
        public bool ManagesUniqueCollection { get; private set; }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        private void Initialize()
        {
            var att = EntityCollectionProviderResolver.Current.GetProviderAttributeByProviderKey(this.Key);
            if (att == null) return;
            
            this.Name = att.Name;
            this.Description = att.Description;
            this.ManagesUniqueCollection = att.ManagesUniqueCollection;
        }
    }
}