namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Xml.Linq;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <inheritdoc/>
    internal class CountriesElement : RawXmlConfigurationElement, IMerchelloCountriesSection
    {
        /// <inheritdoc/>
        public IEnumerable<ICountry> Countries
        {
            get
            {
                var countries = new List<ICountry>();
                
                var xcountries = RawXml.Elements("country").ToArray();

                var xregionsRoot = RawXml.Element("regions");
                if (xregionsRoot == null) throw new ConfigurationErrorsException("Regions element not present in the configuration file");

                var xregions = xregionsRoot.Elements("region").ToArray();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var xc in xcountries)
                {

                    var region = xregions.FirstOrDefault(x => x.Attribute("code").Value == xc.Attribute("code").Value);
                    countries.Add(CreateCountry(xc, region));
                }

                return countries;
            }
        }

        /// <summary>
        /// Creates a <see cref="ICountry"/> from configuration XML data.
        /// </summary>
        /// <param name="xCountry">
        /// The XML configuration element representing the country.
        /// </param>
        /// <param name="xRegion">
        /// The a corresponding XML configuration element representing the region.
        /// </param>
        /// <returns>
        /// The <see cref="ICountry"/>.
        /// </returns>
        private static ICountry CreateCountry(XElement xCountry, XElement xRegion)
        {
            var provinces = xRegion != null ? xRegion.Elements("province").Select(CreateProvince) : Enumerable.Empty<IProvince>();

            var iso = xCountry.Attribute("iso").Value.TryConvertTo<int>();

            return new Country(xCountry.Attribute("code").Value, xCountry.Value, provinces)
                       {
                           Iso = iso.Success ? iso.Result : 0
                       };

        }

        /// <summary>
        /// Creates a <see cref="IProvince"/>.
        /// </summary>
        /// <param name="xr">
        /// The <see cref="XElement"/> representation of the province configuration XML.
        /// </param>
        /// <returns>
        /// The <see cref="IProvince"/>.
        /// </returns>
        private static IProvince CreateProvince(XElement xr)
        {
            return new Province(xr.Attribute("name").Value, xr.Attribute("code").Value);
        }
    }
}