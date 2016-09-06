namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Xml.Linq;

    using Merchello.Core.Configuration.Sections;
    using Merchello.Core.Models.Interfaces;

    /// <inheritdoc/>
    internal class VirtualContentRoutingElement : RawXmlConfigurationElement, IVirtualContentRoutingSection
    {
        /// <inheritdoc/>
        public IEnumerable<IContentFinderRouteBasePath> SlugContentFinderRouteBasePath
        {
            get
            {
                var basePaths = new List<IContentFinderRouteBasePath>();

                if (RawXml == null) return Enumerable.Empty<IContentFinderRouteBasePath>();
                
                    var routes = RawXml.Elements("route").ToArray();

                    basePaths.AddRange(
                        routes.Select(
                            r => 
                            new ContentFinderRouteBasePath
                                {
                                    CultureName = r.Attribute("cultureName").Value,
                                    BasePath = r.Attribute("productSlugPrefix").Value
                                }));
                

                return basePaths;
            }
        }

    }
}