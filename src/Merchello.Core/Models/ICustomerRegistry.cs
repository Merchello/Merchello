using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer registry
    /// </summary>
    public interface ICustomerRegistry : IIdEntity
    {
        /// <summary>
        /// The <see cref="IConsumer"/> key
        /// </summary>
        [DataMember]
        Guid ConsumerKey { get; set; }

        /// <summary>
        /// The registry type field <see cref="ITypeField"/> guid typeKey
        /// </summary>
        [DataMember]
        Guid CustomerRegistryTfKey { get; set; }

        /// <summary>
        /// The <see cref="CustomerRegistryType"/> of the customer registry
        /// </summary>
        [DataMember]
        CustomerRegistryType CustomerRegistryType { get; set; }

        /// <summary>
        /// The <see cref="IPurchaseLineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        IEnumerable<IPurchaseLineItem> Items { get; }


    }
}
