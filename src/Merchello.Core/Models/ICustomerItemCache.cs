using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer item cache
    /// </summary>
    public interface ICustomerItemCache : IIdEntity
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
        Guid ItemCacheTfKey { get; set; }

        /// <summary>
        /// The <see cref="CustomerItemCacheType"/> of the customer registry
        /// </summary>
        [DataMember]
        CustomerItemCacheType CustomerItemCacheType { get; set; }

        /// <summary>
        /// The <see cref="IOrderLineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        LineItemCollection Items { get; }


    }
}
