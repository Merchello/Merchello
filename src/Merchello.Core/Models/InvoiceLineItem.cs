using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
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

        public InvoiceLineItem(Guid containerKey, string name, string sku, decimal amount) 
            : base(containerKey, name, sku, amount)
        {
        }

        public InvoiceLineItem(Guid containerKey, string name, string sku, int quantity, decimal amount) 
            : base(containerKey, name, sku, quantity, amount)
        {
        }

        public InvoiceLineItem(Guid containerKey, LineItemType lineItemType, string name, string sku, int quantity, decimal amount) 
            : base(containerKey, lineItemType, name, sku, quantity, amount)
        {
        }

        public InvoiceLineItem(Guid containerKey, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(containerKey, lineItemType, name, sku, quantity, amount, extendedData)
        {
        }

        public InvoiceLineItem(Guid containerKey, Guid lineItemTfKey, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(containerKey, lineItemTfKey, name, sku, quantity, amount, extendedData)
        {
        }
    }
}