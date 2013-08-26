using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer basket
    /// </summary>
    public interface IBasket : IIdEntity
    {
        /// <summary>
        /// The <see cref="IConsumer"/> key
        /// </summary>
        [DataMember]
        Guid ConsumerKey { get; set; }

        /// <summary>
        /// The basket <see cref="ITypeField"/> guid typeKey
        /// </summary>
        [DataMember]
        Guid BasketTypeFieldKey { get; set; }


        /// <summary>
        /// The <see cref="BasketType"/> of the basket
        /// </summary>
        [DataMember]
        BasketType BasketType { get; set; }
    }
}
