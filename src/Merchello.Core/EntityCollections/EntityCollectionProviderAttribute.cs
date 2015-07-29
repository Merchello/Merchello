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
        /// <param name="name">
        /// The name.
        /// </param>
        public EntityCollectionProviderAttribute(string name)
            : this(name, string.Empty)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public EntityCollectionProviderAttribute(string name, string description)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");

            this.Name = name;
            this.Description = description;
        }


        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }
    }
}