namespace Merchello.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a campaign
    /// </summary>
    internal class CampaignSettings : CampaignSettingsBase, ICampaignSettings
    {
        /// <summary>
        /// Gets or sets the collection of campaign activity settings.
        /// </summary>
        public IEnumerable<ICampaignActivitySettings> ActivitySettings
        {
            get; 
            internal set;
        }
    }
}