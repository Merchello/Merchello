namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Defines a Merchello ShipMethod object interface
    /// </summary>
    public interface IShipMethod : IGatewayProviderMethod
    {            
        /// <summary>
        /// Gets or sets the name for the method
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets the key associated with the gateway provider for the Ship Method
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }
            
        /// <summary>
        /// Gets the key associated with the ship country for the method
        /// </summary>
        [DataMember]
        Guid ShipCountryKey { get; }
            
        /// <summary>
        /// Gets or sets the surcharge for the method
        /// </summary>
        /// TODO this has not been implemented
        [DataMember]
        decimal Surcharge { get; set; }
            
        /// <summary>
        /// The service code (generally defined by the provider) for the ShipMethod
        /// </summary>
        [DataMember]
        string ServiceCode { get; set;}

        /// <summary>
        /// True/false indicating whether or not this shipmethod is taxable
        /// </summary>
        [DataMember]
        bool Taxable { get; set; }

        /// <summary>
        /// Stores province settings for this ship method
        /// </summary>
        [DataMember]
        ProvinceCollection<IShipProvince> Provinces { get; set; }
    }
}



