using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Line Item
    /// </summary>
    public interface ILineItem : IHasExtendedData, IEntity
    {
        /// <summary>
        /// The key of the container collection
        /// </summary>
        [DataMember]
        Guid ContainerKey { get; set; }

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
        /// The price for the line item
        /// </summary>
        [DataMember]
        decimal Price { get; set; }

        /// <summary>
        /// True/false indicating whether or not this line item has been exported to an external system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }


        /// <summary>
        /// The line item type
        /// </summary>
        [DataMember]
        LineItemType LineItemType { get; }

        /// <summary>
        /// The total price of the line item (quantity * price)
        /// </summary>
        [IgnoreDataMember]
        decimal TotalPrice { get; }

        /// <summary>
        /// Accept for visitor operations
        /// </summary>
        /// <param name="vistor"><see cref="ILineItemVisitor"/></param>
        void Accept(ILineItemVisitor vistor);
    }
}