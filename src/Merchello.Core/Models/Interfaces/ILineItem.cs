namespace Merchello.Core.Models
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// Represents a Line Item
    /// </summary>
    public interface ILineItem : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the key of the container collection
        /// </summary>
        Guid ContainerKey { get; set; }

        /// <summary>
        /// Gets or sets the line item type field key (<see cref="ITypeField"/>.TypeKey) for the registry item
        /// </summary>
        Guid LineItemTfKey { get; set; }

        /// <summary>
        /// Gets or sets SKU for the line item
        /// </summary>
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name for the line item
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity for the line item
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price for the line item
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this line item has been exported to an external system
        /// </summary>
        /// <remarks>
        /// This property is not used by Merchello but is useful in some implementations.
        /// </remarks>
        bool Exported { get; set; }


        /// <summary>
        /// Gets line item type
        /// </summary>
        LineItemType LineItemType { get; }

        /// <summary>
        /// Gets total price of the line item (quantity * price)
        /// </summary>
        [IgnoreDataMember]
        decimal TotalPrice { get; }

        /// <summary>
        /// Accept for visitor operations
        /// </summary>
        /// <param name="vistor">The <see cref="ILineItemVisitor"/></param>
        void Accept(ILineItemVisitor vistor);
    }
}