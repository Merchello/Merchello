using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    public interface ILineItem : IEntity
    {
        /// <summary>
        /// The key of the container collection
        /// </summary>
        [DataMember]
        Guid ContainerKey { get; }

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

        /// <summary>
        /// True/false indicating whether or not this line item has been exported to an external system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }

        /// <summary>
        /// A collection to store custom/extended data for the line item
        /// </summary>
        [DataMember]
        ExtendedDataCollection ExtendedData { get; }

        /// <summary>
        /// Accept for visitor operations
        /// </summary>
        /// <param name="vistor"><see cref="ILineItemVisitor"/></param>
        void Accept(ILineItemVisitor vistor);
    }
}