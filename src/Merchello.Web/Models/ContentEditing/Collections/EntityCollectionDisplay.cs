namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence;
    using Merchello.Web.Models.ContentEditing.Operations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents and entity collection.
    /// </summary>
    [DataContract(Name = "entityCollectionDisplay", Namespace = "")]
    public class EntityCollectionDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [DataMember(Name = "parentKey")]
        public Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember(Name = "entityTfKey")]
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field.
        /// </summary>
        [DataMember(Name = "entityTypeField")]
        public TypeField EntityTypeField { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [DataMember(Name = "entityType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        [DataMember(Name = "providerKey")]
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is filter.
        /// </summary>
        [DataMember(Name = "isFilter")]
        public bool IsFilter { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [DataMember(Name = "extendedData")]
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="EntityCollectionDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class EntityCollectionDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IEntitySpecifiedFilterCollection"/> to <see cref="EntitySpecifiedFilterCollectionDisplay"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntitySpecifiedFilterCollectionDisplay"/>.
        /// </returns>
        public static EntitySpecifiedFilterCollectionDisplay ToEntitySpecificationCollectionDisplay(this IEntitySpecifiedFilterCollection collection)
        {
            return AutoMapper.Mapper.Map<EntitySpecifiedFilterCollectionDisplay>(collection);
        }

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

        /// <summary>
        /// Maps a <see cref="EntityCollectionDisplay"/> to an <see cref="IEntityCollection"/>.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        public static IEntityCollection ToEntityCollection(
            this EntityCollectionDisplay display,
            IEntityCollection destination)
        {
            if (!Guid.Empty.Equals(display.Key)) destination.Key = display.Key;
            destination.Name = display.Name;
            destination.ProviderKey = display.ProviderKey;
            destination.EntityTfKey = display.EntityTfKey;
            destination.ParentKey = display.ParentKey.GetValueOrDefault();
            ((EntityCollection)destination).SortOrder = display.SortOrder;
            ((EntityCollection)destination).ExtendedData = display.ExtendedData.AsExtendedDataCollection();
            destination.IsFilter = display.IsFilter;
            return destination;
        }
    }
}