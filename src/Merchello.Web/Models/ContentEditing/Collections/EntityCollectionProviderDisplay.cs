namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.TypeFields;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents an entity collection provider.
    /// </summary>
    public class EntityCollectionProviderDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field.
        /// </summary>
        public TypeField EntityTypeField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether manages unique collection.
        /// </summary>
        public bool ManagesUniqueCollection { get; set; }

        /// <summary>
        /// Gets or sets the localization name key.
        /// </summary>
        /// <remarks>
        /// e.g. "merchelloProviders/providerNameKey"
        /// </remarks>
        public string LocalizedNameKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the managed collections.
        /// </summary>
        public IEnumerable<EntityCollectionDisplay> ManagedCollections { get; set;   }
    }

    /// <summary>
    /// The entity collection provider display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class EntityCollectionProviderDisplayExtensions
    {
        /// <summary>
        /// The to entity collection provider display.
        /// </summary>
        /// <param name="att">
        /// The provider attribute.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderDisplay"/>.
        /// </returns>
        public static EntityCollectionProviderDisplay ToEntityCollectionProviderDisplay(this EntityCollectionProviderAttribute att)
        {
            return AutoMapper.Mapper.Map<EntityCollectionProviderDisplay>(att);
        }
    }
}