using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;


namespace Merchello.Core.Models
{
    public interface IInvoiceItem : IItemization, IIdEntity
    {
        /// <summary>
        /// The parent Id of the InvoiceItem
        /// </summary>
        [DataMember]
        int? ParentId { get; set; }

        /// <summary>
        /// The Invoice that this invoice item is associated with
        /// </summary>
        [DataMember]       
        int InvoiceId { get; set; }

        /// <summary>
        /// The shipment this invoice item is associated with
        /// </summary>
        [DataMember]
        int ShipmentId { get; set; }

        [DataMember]
        IInvoiceItemItemization Itemization { get; }

        //TODO: RSS create a property shim to get/set InvoiceItemType via enumeration name
        ///// <summary>
        ///// The type of this invoice item
        ///// </summary>
        //[IgnoreDataMember]
        //InvoiceItemTypeField InvoiceItemType { get; set; }

        /// <summary>
        /// The Sku of the invoice item
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// The name or description of the invoice item
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The quantity of the the invoice item
        /// </summary>
        [DataMember]
        int BaseQuantity { get; set; }

        /// <summary>
        /// The unit of measure of the invoice item
        /// </summary>
        [DataMember]
        int UnitOfMeasure { get; set; }

        /// <summary>
        /// Flag to indicate whether or not this item has been exported to another system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }


    }


}
