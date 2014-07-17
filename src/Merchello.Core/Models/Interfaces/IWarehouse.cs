using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Warehouse object interface
    /// </summary>
    public interface IWarehouse : IEntity
    {            
        /// <summary>
        /// Gets or sets the name for the Warehouse
        /// </summary>
        [DataMember]
        string Name { get; set; }
            
        /// <summary>
        /// Gets or sets the address1 for the Warehouse
        /// </summary>
        [DataMember]
        string Address1 { get; set; }
            
        /// <summary>
        /// Gets or sets the address2 for the Warehouse
        /// </summary>
        [DataMember]
        string Address2 { get; set; }
            
        /// <summary>
        /// Gets or sets the locality for the Warehouse
        /// </summary>
        [DataMember]
        string Locality { get; set; }
            
        /// <summary>
        /// Gets or sets the region for the Warehouse
        /// </summary>
        [DataMember]
        string Region { get; set; }
            
        /// <summary>
        /// Gets or sets the postalCode for the Warehouse
        /// </summary>
        [DataMember]
        string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code of the warehouse
        /// </summary>
        [DataMember]
        string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the warehouse
        /// </summary>
        [DataMember]
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the contact email address of the warehouse
        /// </summary>
        [DataMember]
        string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this warehouse is the primary (or default) warehouse
        /// </summary>
        [DataMember]
        bool IsDefault { get; set; }

        /// <summary>
        /// Gets the warehouse catalogs.
        /// </summary>
        IEnumerable<IWarehouseCatalog> WarehouseCatalogs { get; }
    }
}



