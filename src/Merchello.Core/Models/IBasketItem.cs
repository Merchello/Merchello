using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a BasketItem
    /// </summary>
    public interface IBasketItem : IIdEntity
    {
        /// <summary>
        /// The parentId of the basket item
        /// </summary>
        [DataMember]
        int? ParentId { get; set; }

        /// <summary>
        /// The basketId of the basket item
        /// </summary>
        [DataMember]
        int BasketId { get; }

        /// <summary>
        /// The invoiceItemTypeFieldKey (<see cref="ITypeField"/>.TypeKey) for the basket item
        /// </summary>
        [DataMember]
        Guid InvoiceItemTypeFieldKey { get; set; }

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
        /// The unitOfMeasureMultiplier for the basket item
        /// </summary>
        [DataMember]
        int UnitOfMeasureMultiplier { get; set; }

        /// <summary>
        /// The amount for the basket item
        /// </summary>
        [DataMember]
        decimal Amount { get; set; }
    }
}
