namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a ship method.
    /// </summary>
    public interface IShipMethod : IGatewayProviderMethod
    {            
        /// <summary>
        /// Gets or sets the name for the method
        /// </summary>
        
        string Name { get; set; }

        /// <summary>
        /// Gets the key associated with the gateway provider for the Ship Method
        /// </summary>
        
        Guid ProviderKey { get; }
            
        /// <summary>
        /// Gets the key associated with the ship country for the method
        /// </summary>
        
        Guid ShipCountryKey { get; }
            
        /// <summary>
        /// Gets or sets the surcharge for the method
        /// </summary>
        /// TODO this has not been implemented
        
        decimal Surcharge { get; set; }
            
        /// <summary>
        /// The service code (generally defined by the provider) for the ShipMethod
        /// </summary>
        
        string ServiceCode { get; set;}

        /// <summary>
        /// True/false indicating whether or not this shipmethod is taxable
        /// </summary>
        
        bool Taxable { get; set; }

        /// <summary>
        /// Stores province settings for this ship method
        /// </summary>
        
        //ProvinceCollection<IShipProvince> Provinces { get; set; }
    }
}



