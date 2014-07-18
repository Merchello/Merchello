namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an invoice line item
    /// </summary>
    /// <remarks>
    /// Needed for typed query mapper
    /// </remarks>
    [Serializable]
    [DataContract(IsReference = true)]
    public class InvoiceLineItem : LineItemBase, IInvoiceLineItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public InvoiceLineItem(string name, string sku, decimal amount) 
            : base(name, sku, amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public InvoiceLineItem(string name, string sku, int quantity, decimal amount) 
            : base(name, sku, quantity, amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItem"/> class.
        /// </summary>
        /// <param name="lineItemType">
        /// The line item type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        public InvoiceLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal price) 
            : base(lineItemType, name, sku, quantity, price)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItem"/> class.
        /// </summary>
        /// <param name="lineItemType">
        /// The line item type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public InvoiceLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData) 
            : base(lineItemType, name, sku, quantity, price, extendedData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItem"/> class.
        /// </summary>
        /// <param name="lineItemTfKey">
        /// The line item type field key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public InvoiceLineItem(Guid lineItemTfKey, string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData) 
            : base(lineItemTfKey, name, sku, quantity, price, extendedData)
        {
        }
    }
}