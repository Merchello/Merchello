namespace Merchello.Core.Marketing
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The campaign activity base.
    /// </summary>
    public abstract class CampaignActivityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivityBase"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        protected CampaignActivityBase(CampaignActivitySettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");

            this.Settings = settings;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public CampaignActivitySettings Settings { get; private set;  }
    }
}