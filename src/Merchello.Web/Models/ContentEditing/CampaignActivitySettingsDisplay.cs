namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models.TypeFields;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The campaign activity settings display.
    /// </summary>
    public class CampaignActivitySettingsDisplay
    {
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
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the campaign key.
        /// </summary>
        public Guid CampaignKey { get; set; }

        /// <summary>
        /// Gets or sets the campaign activity type field key.
        /// </summary>
        public Guid CampaignActivityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the campaign activity type field.
        /// </summary>
        public TypeField CampaignActivityTypeField { get; set; }

        /// <summary>
        /// Gets or sets the campaign activity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CampaignActivityType CampaignActivityType { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
    }
}