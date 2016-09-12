namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class SalesElement : ConfigurationElement, ISalesSection
    {
        /// <inheritdoc/>
        bool ISalesSection.AlwaysApproveOrderCreation
        {
            get
            {
                return AlwaysApproveOrderCreation;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("alwaysApproveOrderCreation")]
        internal InnerTextConfigurationElement<bool> AlwaysApproveOrderCreation
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["alwaysApproveOrderCreation"];
            }
        }
    }
}