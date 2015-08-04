namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents and entity collection.
    /// </summary>
    public class EntityCollectionDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        public Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="EntityCollectionDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class EntityCollectionDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IEntityCollection"/> to <see cref="EntityCollectionDisplay"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionDisplay"/>.
        /// </returns>
        public static EntityCollectionDisplay ToEntityCollectionDisplay(this IEntityCollection collection)
        {
            return AutoMapper.Mapper.Map<EntityCollectionDisplay>(collection);
        }
    }
}