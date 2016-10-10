namespace Merchello.Core.Models
{
    using System;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents shipping method.
    /// </summary>
    public interface IShipMethod : IGatewayProviderMethod
    {            
        /// <summary>
        /// Gets or sets the name for the shipping method.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the key associated with the gateway provider.
        /// </summary>
        Guid ProviderKey { get; }
            
        /// <summary>
        /// Gets the key associated with the ship country.
        /// </summary>
        Guid ShipCountryKey { get; }
            
        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        /// <remarks>
        /// Currently not being used.
        /// </remarks>
        decimal Surcharge { get; set; }
            
        /// <summary>
        /// Gets or sets the service code (generally defined by the provider).
        /// </summary>
        string ServiceCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this shipping method is taxable.
        /// </summary>
        bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets the province collection.
        /// </summary>
        ProvinceCollection<IShipProvince> Provinces { get; set; }
    }
}



