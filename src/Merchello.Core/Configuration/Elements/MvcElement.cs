namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;
    
    /// <inheritdoc/>
    internal class MvcElement : ConfigurationElement, IMvcSection
    {
        /// <inheritdoc/>
        IViewsSection IMvcSection.Views
        {
            get;
        }

        /// <inheritdoc/>
        IVirtualContentSection IMvcSection.VirtualContent
        {
            get
            {
                return this.VirtualContent;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("virtualContent", IsRequired = false)]
        internal VirtualContentElement VirtualContent
        {
            get
            {
                return (VirtualContentElement)this["virtualContent"];
            }
        }
    }
}