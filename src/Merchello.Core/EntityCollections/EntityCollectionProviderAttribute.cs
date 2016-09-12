namespace Merchello.Core.EntityCollections
{
    using System;

    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// An attribute to decorate entity collection providers for resolution.
    /// </summary>
    public class EntityCollectionProviderAttribute : Attribute, IEntityCollectionProviderMeta
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
            : this(key, entityTfKey, name, description, true, string.Empty)
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
        /// <param name="localizedNameKey">
        /// The localization key for the name of the provide.  Used in cases where provider is referenced in Merchello back office tree.
        /// </param>
        /// <param name="editorView">
        /// Can be used to override the editor dialog
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
            bool managesUniqueCollection, 
            string localizedNameKey = "",
            string editorView = "")
        {
            Ensure.ParameterNotNullOrEmpty(key, "key");
            Ensure.ParameterNotNullOrEmpty(entityTfKey, "entityTfKey");
            Ensure.ParameterNotNullOrEmpty(name, "name");
            this.Key = new Guid(key);
            this.EntityTfKey = new Guid(entityTfKey);
            this.Name = name;
            this.Description = description;
            this.ManagesUniqueCollection = managesUniqueCollection;
            this.LocalizedNameKey = localizedNameKey;
            this.EditorView = editorView;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key { get; private set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets the localization name key.
        /// </summary>
        /// <remarks>
        /// e.g. "merchelloProviders/providerNameKey"
        /// </remarks>
        public string LocalizedNameKey { get; set; }

        /// <summary>
        /// Gets a value indicating whether manages unique collection.
        /// </summary>
        public bool ManagesUniqueCollection { get; private set; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public EntityType EntityType
        {
            get
            {
                return EnumTypeFieldConverter.EntityType.GetTypeField(EntityTfKey);
            }
        }

        /// <summary>
        /// Gets the relative path to the editor view html
        /// </summary>
        public string EditorView { get; private set; }
    }
}