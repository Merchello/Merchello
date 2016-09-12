namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class MigrationsElement : ConfigurationElement, IMigrationsSection
    {
        /// <inheritdoc/>
        bool IMigrationsSection.AutoUpdateDbSchema
        {
            get
            {
                return this.AutoUpdateDbSchema;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("autoUpdateDbSchema")]
        internal InnerTextConfigurationElement<bool> AutoUpdateDbSchema
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["autoUpdateDbSchema"];
            }
        } 
    }
}