namespace Merchello.Core.Models
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The CampaignActivity interface.
    /// </summary>
    public interface ICampaignActivitySettings : ICampaignBase, IEntity
    {
        /// <summary>
        /// Gets the campaign key.
        /// </summary>
        Guid CampaignKey { get; }

        /// <summary>
        /// Gets the campaign activity type field key.
        /// </summary>
        Guid CampaignActivityTfKey { get; }

        /// <summary>
        /// Gets the campaign activity type.
        /// </summary>
        CampaignActivityType CampaignActivityType { get; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        DateTime EndDate { get; set; }

        /// <summary>
        /// Gets the extended data.
        /// </summary>
        ExtendedDataCollection ExtendedData { get; }
    }
}