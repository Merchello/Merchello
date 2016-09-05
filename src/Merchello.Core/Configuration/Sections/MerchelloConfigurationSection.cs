namespace Merchello.Core.Configuration.Sections
{
    using System.Configuration;

    /// <inheritdoc/>
    public abstract class MerchelloConfigurationSection : ConfigurationSection, IMerchelloConfigurationSection
    {
        /// <summary>
        /// Gets a value indicating whether the section actually is in the configuration file.
        /// </summary>
        protected bool IsPresent
        {
            get
            {
                return this.ElementInformation.IsPresent;
            }
        }
    }
}