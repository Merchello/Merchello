namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.BackOffice;
    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class BackOfficeElement : ConfigurationElement, IBackOfficeSection
    {
        /// <inheritdoc/>
        IEnumerable<IDashboardTreeNode> IBackOfficeSection.Trees
        {
            get
            {
                return this.Trees.GetTrees();
            }
        }

        /// <inheritdoc/>
        bool IBackOfficeSection.EnableProductOptionUiElementSelection
        {
            get
            {
                return this.EnableProductOptionUiElementSelection;
            }
        }

        /// <inheritdoc/>
        IEnumerable<KeyValuePair<string, string>> IBackOfficeSection.ProductOptionUi
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("trees", IsRequired = true)]
        internal BackOfficeTreesElement Trees
        {
            get
            {
                return (BackOfficeTreesElement)this["trees"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("enableProductOptionUiElementSelection", IsRequired = false, DefaultValue = false)]
        internal InnerTextConfigurationElement<bool> EnableProductOptionUiElementSelection
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["enableProductOptionUiElementSelection"];
            }
        }
    }
}