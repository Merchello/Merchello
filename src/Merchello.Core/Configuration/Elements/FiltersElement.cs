namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.BackOffice;
    using Merchello.Core.Configuration.Sections;

    /// <summary>
    /// Represents the filters configuration element.
    /// </summary>
    internal class FiltersElement : RawXmlConfigurationElement, IFiltersSection
    {
        /// <summary>
        /// Gets the entity collection provider references associated with products.
        /// </summary>
        public IEnumerable<IDashboardTreeNodeKeyLink> Products
        {
            get
            {
                var xproducts = this.RawXml.Element("products");
                if (xproducts == null) return Enumerable.Empty<IDashboardTreeNodeKeyLink>();

                var values = xproducts.Elements("entityCollectionProvider");
                return values.Select(val => val.AsDashboardTreeNodeKeyLink());
            }
        }
    }
}