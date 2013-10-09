using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    public interface ILineItem : IIdEntity
    {

        /// <summary>
        /// The ContainerId of the container collection
        /// </summary>
        [DataMember]
        int ContainerId { get; }

        /// <summary>
        /// The line item type field key (<see cref="ITypeField"/>.TypeKey) for the registry item
        /// </summary>
        [DataMember]
        Guid LineItemTfKey { get; set; }

        /// <summary>
        /// The sku for the line item
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// The name for the line item
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The Quantity for the line item
        /// </summary>
        [DataMember]
        int Quantity { get; set; }

        /// <summary>
        /// The amount for the line item
        /// </summary>
        [DataMember]
        decimal Amount { get; set; } 
    }
}