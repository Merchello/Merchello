using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello ShipMethod object interface
    /// </summary>
    public interface IShipMethod : IEntity
    {            
        /// <summary>
        /// The name for the ShipMethod
        /// </summary>
        [DataMember]
        string Name { get; set;}

        /// <summary>
        /// The key associated with the gateway provider for the Ship Method
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; set;}
            
        /// <summary>
        /// The shipMethodTypeFieldKey for the ShipMethod
        /// </summary>
        [DataMember]
        Guid ShipMethodTfKey { get; set;}
            
        /// <summary>
        /// The surcharge for the ShipMethod
        /// </summary>
        [DataMember]
        decimal Surcharge { get; set;}
            
        /// <summary>
        /// The service code (generally defined by the provider) for the ShipMethod
        /// </summary>
        [DataMember]
        string ServiceCode { get; set;}

        /// <summary>
        /// Stores province settings for this ship method
        /// </summary>
        [DataMember]
        ProvinceCollection<IShipProvince> Provinces { get; set; }
    }
}



