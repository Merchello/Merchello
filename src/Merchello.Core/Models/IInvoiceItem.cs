using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello InvoiceItem object interface
    /// </summary>
    public interface IInvoiceItem : IIdEntity
    {
            
            /// <summary>
            /// The parentId of the invoice item
            /// </summary>
            [DataMember]
            int? ParentId { get; }
            
            /// <summary>
            /// The invoiceId of the invoice item
            /// </summary>
            [DataMember]
            int InvoiceId { get; }

            /// <summary>
            /// The invoiceItemTfKey (<see cref="ITypeField"/>.TypeKey) for the invoice item
            /// </summary>
            [DataMember]
            Guid InvoiceItemTfKey { get; set;}
            
            /// <summary>
            /// The sku  of the invoice item
            /// </summary>
            [DataMember]
            string Sku { get; set;}
            
            /// <summary>
            /// The name  of the invoice item
            /// </summary>
            [DataMember]
            string Name { get; set;}
            
            /// <summary>
            /// The base quantity  of the invoice item
            /// </summary>
            [DataMember]
            int Quantity { get; set;}
                        
            
            /// <summary>
            /// The amount  of the invoice item
            /// </summary>
            [DataMember]
            decimal Amount { get; set;}

            /// <summary>
            /// True/false indicating whether or not this invoice has been exported to an external system
            /// </summary>
            [DataMember]
            bool Exported { get; set;}

            
    }
}



