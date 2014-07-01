namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The ship method display.
    /// </summary>
    public class ShipMethodDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the ship country key.
        /// </summary>
        public Guid ShipCountryKey { get; set; }

        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        public decimal Surcharge { get; set; }

        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether taxable.
        /// </summary>
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets the provinces.
        /// </summary>
        public IEnumerable<ShipProvinceDisplay> Provinces { get; set; }
    }
}
