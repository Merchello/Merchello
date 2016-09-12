namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Xml.Linq;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Mvc;
    using Merchello.Core.Configuration.Sections;
    using Merchello.Core.Models.Interfaces;

    /// <inheritdoc/>
    internal class VirtualContentRoutingElement : RawXmlConfigurationElement, IVirtualContentRoutingSection
    {
        /// <inheritdoc/>
        public IEnumerable<ICultureRouteBasePath> ProductSlugRoutes
        {
            get
            {
                var basePaths = new List<ICultureRouteBasePath>();

                if (RawXml == null) return Enumerable.Empty<ICultureRouteBasePath>();
                
                    var routes = RawXml.Elements("route").ToArray();

                    basePaths.AddRange(
                        routes.Select(
                            r => 
                            new CultureRouteBasePath
                                {
                                    CultureName = r.Attribute("cultureName").Value,
                                    BasePath = r.Attribute("productSlugPrefix").Value
                                }));
                

                return basePaths;
            }
        }

    }
}