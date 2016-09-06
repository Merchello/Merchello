namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    public class VirtualContentElement : ConfigurationElement, IVirtualContentSection
    {
        /// <inheritdoc/>
        IVirtualContentRoutingSection IVirtualContentSection.Routing
        {
            get
            {
                return this.Routing;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("routing", IsRequired = false)]
        internal VirtualContentRoutingElement Routing
        {
            get
            {
                return (VirtualContentRoutingElement)this["routing"];
            }
        }
    }
}