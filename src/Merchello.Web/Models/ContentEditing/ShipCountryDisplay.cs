namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The ship country display.
    /// </summary>
    public class ShipCountryDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the province label.
        /// </summary>
        public string ProvinceLabel { get; set; }

        /// <summary>
        /// Gets or sets the catalog key.
        /// </summary>
        public Guid CatalogKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has provinces.
        /// </summary>
        public bool HasProvinces { get; set; }

        /// <summary>
        /// Gets or sets the provinces.
        /// </summary>
        public IEnumerable<ProvinceDisplay> Provinces { get; set; }
    }
}
