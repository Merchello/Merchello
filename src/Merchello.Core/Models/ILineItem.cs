using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    public interface ILineItem : IIdEntity
    {
        /// <summary>
        /// The id of the list item parent
        /// </summary>
        [DataMember]
        int? ParentId { get; set; }

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
        /// The sku for the basket item
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// The name for the basket item
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The baseQuantity for the basket item
        /// </summary>
        [DataMember]
        int BaseQuantity { get; set; }

        /// <summary>
        /// The amount for the basket item
        /// </summary>
        [DataMember]
        decimal Amount { get; set; } 
    }
}