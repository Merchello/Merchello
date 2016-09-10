namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class ViewsElement : ConfigurationElement, IViewsSection
    {
        /// <inheritdoc/>
        string IViewsSection.DefaultPath
        {
            get
            {
                return this.BasePath;
            }
        }

        /// <inheritdoc/>
        string IViewsSection.Notifications
        {
            get
            {
                return this.Notifications;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("defaultPath", IsRequired = true)]
        internal InnerTextConfigurationElement<string> BasePath
        {
            get
            {
                return (InnerTextConfigurationElement<string>)this["basePath"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("notifications", IsRequired = true)]
        internal InnerTextConfigurationElement<string> Notifications
        {
            get
            {
                return (InnerTextConfigurationElement<string>)this["notifications"];
            }
        }

    }
}