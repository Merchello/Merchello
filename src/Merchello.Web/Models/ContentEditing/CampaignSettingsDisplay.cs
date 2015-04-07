namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Cache;
    using Merchello.Core.Models;

    /// <summary>
    /// The campaign settings display.
    /// </summary>
    public class CampaignSettingsDisplay
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
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the activity settings.
        /// </summary>
        public IEnumerable<CampaignActivitySettingsDisplay> ActivitySettings { get; set; }
    }

    /// <summary>
    /// Utility mapping extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CampaignSettingsDisplayExtensions
    {
        /// <summary>
        /// Maps a <see cref="ICampaignSettings"/> to <see cref="CampaignSettingsDisplay"/>.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="CampaignSettingsDisplay"/>.
        /// </returns>
        public static CampaignSettingsDisplay ToCampaignSettingsDisplay(this ICampaignSettings settings)
        {
            return AutoMapper.Mapper.Map<CampaignSettingsDisplay>(settings);
        }

        /// <summary>
        /// Maps a <see cref="CampaignSettingsDisplay"/> to <see cref="ICampaignSettings"/>.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignSettings"/>.
        /// </returns>
        public static ICampaignSettings ToCampaignSettings(this CampaignSettingsDisplay settings, ICampaignSettings destination)
        {
            destination.Name = settings.Name;
            destination.Alias = settings.Alias;

            return destination;
        }
    }
}