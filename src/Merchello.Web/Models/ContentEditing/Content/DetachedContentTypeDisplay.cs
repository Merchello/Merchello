namespace Merchello.Web.Models.ContentEditing.Content
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.TypeFields;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The detached content type display.
    /// </summary>
    public class DetachedContentTypeDisplay
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
        /// Gets or sets the <see cref="UmbContentTypeDisplay"/>.
        /// </summary>
        public UmbContentTypeDisplay UmbContentType { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EntityTypeField"/>.
        /// </summary>
        public TypeField EntityTypeField { get; set; }
    }

    /// <summary>
    /// Utility mapping extensions for <see cref="DetachedContentTypeDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class DetachedContentTypeDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IDetachedContentType"/> to <see cref="DetachedContentTypeDisplay"/>.
        /// </summary>
        /// <param name="dtc">
        /// The <see cref="IDetachedContentType"/>.
        /// </param>
        /// <returns>
        /// The <see cref="DetachedContentTypeDisplay"/>.
        /// </returns>
        public static DetachedContentTypeDisplay ToDetachedContentTypeDisplay(this IDetachedContentType dtc)
        {
            return AutoMapper.Mapper.Map<DetachedContentTypeDisplay>(dtc);
        }

        /// <summary>
        /// Maps <see cref="DetachedContentTypeDisplay"/> to <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public static IDetachedContentType ToDetachedContentType(this DetachedContentTypeDisplay contentType, IDetachedContentType destination)
        {
            destination.Name = contentType.Name;
            destination.Description = contentType.Description;
            return destination;
        }
    }
}