namespace Merchello.Core.EntityCollections
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// An attribute to decorate entity collection providers for resolution.
    /// </summary>
    public class EntityCollectionProviderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public EntityCollectionProviderAttribute(string key, string entityTfKey, string name)
            : this(key, entityTfKey, name, string.Empty)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public EntityCollectionProviderAttribute(string key, string entityTfKey, string name, string description)
            : this(key, entityTfKey, name, description, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="managesUniqueCollection">
        /// The manages unique collection.
        /// </param>
        /// <remarks>
        /// If managesUniqueCollection is true, the boot manager will automatically add the collection to the merchEntityCollection table if it does not exist.
        /// Likewise, if the provider is removed, it will remove itself from the merchEntityCollection table
        /// </remarks>
        public EntityCollectionProviderAttribute(
            string key,
            string entityTfKey,
            string name,
            string description,
            bool managesUniqueCollection)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(entityTfKey, "entityTfKey");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            this.Key = new Guid(key);
            this.EntityTfKey = new Guid(entityTfKey);
            this.Name = name;
            this.Description = description;
            this.ManagesUniqueCollection = managesUniqueCollection;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key { get; private set; }

        /// <summary>
        /// Gets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; private set; }

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
    }
}