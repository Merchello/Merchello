using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Warehouse object interface
    /// </summary>
    public interface IWarehouse : IIdEntity
    {
            
            /// <summary>
            /// The name for the Warehouse
            /// </summary>
            [DataMember]
            string Name { get; set;}
            
            /// <summary>
            /// The address1 for the Warehouse
            /// </summary>
            [DataMember]
            string Address1 { get; set;}
            
            /// <summary>
            /// The address2 for the Warehouse
            /// </summary>
            [DataMember]
            string Address2 { get; set;}
            
            /// <summary>
            /// The locality for the Warehouse
            /// </summary>
            [DataMember]
            string Locality { get; set;}
            
            /// <summary>
            /// The region for the Warehouse
            /// </summary>
            [DataMember]
            string Region { get; set;}
            
            /// <summary>
            /// The postalCode for the Warehouse
            /// </summary>
            [DataMember]
            string PostalCode { get; set;}
    }
}



