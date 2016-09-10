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
            get
            {
                return this.Views;
            }
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
        [ConfigurationProperty("views", IsRequired = true)]
        internal ViewsElement Views
        {
            get
            {
                return (ViewsElement)this["views"];
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