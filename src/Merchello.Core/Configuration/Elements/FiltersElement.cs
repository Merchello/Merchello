namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;

    public class FiltersElement : ConfigurationElement, IFiltersSection
    {
        public IEnumerable<object> ProductFilterProviders { get; }
    }
}