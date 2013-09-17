using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello ShipMethod object interface
    /// </summary>
    public interface IShipMethod : IIdEntity
    {
            
            /// <summary>
            /// The name for the ShipMethod
            /// </summary>
            [DataMember]
            string Name { get; set;}
            
            /// <summary>
            /// The gatewayAlias for the ShipMethod
            /// </summary>
            [DataMember]
            int GatewayAlias { get; set;}
            
            /// <summary>
            /// The shipMethodTypeFieldKey for the ShipMethod
            /// </summary>
            [DataMember]
            Guid ShipMethodTypeFieldKey { get; set;}
            
            /// <summary>
            /// The surcharge for the ShipMethod
            /// </summary>
            [DataMember]
            decimal Surcharge { get; set;}
            
            /// <summary>
            /// The serviceCode for the ShipMethod
            /// </summary>
            [DataMember]
            string ServiceCode { get; set;}
    }
}



