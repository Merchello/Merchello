namespace Merchello.Core.Models
{
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Marker interface for a Marketing Campaign Settings
    /// </summary>
    public interface ICampaignSettings : ICampaignSettingsBase, IEntity
    {
        /// <summary>
        /// Gets the collection campaign activities settings.
        /// </summary>
        IEnumerable<ICampaignActivitySettings> ActivitySettings { get; }
    }
}