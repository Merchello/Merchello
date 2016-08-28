namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Line Item
    /// </summary>
    public interface ILineItem : IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the key of the container collection
        /// </summary>
        [DataMember]
        Guid ContainerKey { get; set; }

        /// <summary>
        /// Gets or sets the line item type field key for the item
        /// </summary>
        [DataMember]
        Guid LineItemTfKey { get; set; }

        /// <summary>
        /// Gets or sets the sku for the line item
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the name for the line item
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity for the line item
        /// </summary>
        [DataMember]
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price for the line item
        /// </summary>
        [DataMember]
        decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this line item has been exported to an external system
        /// </summary>
        /// <remarks>
        /// Not by Merchello internally and can be safely used for custom implementations
        /// </remarks>
        [DataMember]
        bool Exported { get; set; }


        /// <summary>
        /// Gets the <see cref="LineItemType"/>
        /// </summary>
        [DataMember]
        LineItemType LineItemType { get; }

        /// <summary>
        /// Gets the total price of the line item (quantity * price)
        /// </summary>
        [IgnoreDataMember]
        decimal TotalPrice { get; }

        /// <summary>
        /// Accept for visitor operations
        /// </summary>
        /// <param name="vistor">The <see cref="ILineItemVisitor"/> class to be accepted</param>
        void Accept(ILineItemVisitor vistor);
    }
}