namespace Merchello.Web.Models.ContentEditing.Content
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models.DetachedContent;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Umbraco.Core.Models;

    /// <summary>
    /// Stores information from <see cref="IContentType"/>.
    /// </summary>
    public class UmbContentTypeDisplay
    {
        /// <summary>
        /// Gets or sets the content type id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Umbraco's unique key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the content type alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the default template id.
        /// </summary>
        public int DefaultTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        public IEnumerable<string> Tabs { get; set; }

        /// <summary>
        /// Gets or sets the allowed templates.
        /// </summary>
        public IEnumerable<UmbTemplateDisplay> AllowedTemplates { get; set; } 
    }

    /// <summary>
    /// The embedded content type display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class UmbContentTypeDisplayExtensions
    {
        /// <summary>
        /// The to embedded content type display.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <returns>
        /// The <see cref="UmbContentTypeDisplay"/>.
        /// </returns>
        public static UmbContentTypeDisplay ToUmbContentTypeDisplay(this IContentType contentType)
        {
            return AutoMapper.Mapper.Map<UmbContentTypeDisplay>(contentType);
        }

        /// <summary>
        /// Maps a <see cref="UmbContentTypeDisplay"/> to <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="umbContentType">
        /// The Umbraco content type.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public static IDetachedContentType ToDetachedContentType(this UmbContentTypeDisplay umbContentType, IDetachedContentType destination)
        {
            destination.Name = umbContentType.Name;
            destination.ContentTypeKey = umbContentType.Key;
            return destination;
        } 
    }
}