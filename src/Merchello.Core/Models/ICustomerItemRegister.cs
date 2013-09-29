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
    public interface ICustomerItemRegister : IIdEntity
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
        Guid CustomerRegisterTfKey { get; set; }

        /// <summary>
        /// The <see cref="CustomerItemRegisterType"/> of the customer registry
        /// </summary>
        [DataMember]
        CustomerItemRegisterType CustomerItemRegisterType { get; set; }

        /// <summary>
        /// The <see cref="IOrderLineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        LineItemCollection Items { get; }


    }
}
