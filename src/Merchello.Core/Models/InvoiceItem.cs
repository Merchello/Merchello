using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    internal class InvoiceItem : IdEntity, IInvoiceItem
    {
        private readonly int? _parentId;
        private readonly int _invoiceId;
        private Guid _invoiceItemTfKey;
        private string _sku;
        private string _name;
        private int _quantity;
        private decimal _amount;
        private bool _exported;

        public InvoiceItem(IInvoice invoice, InvoiceItemType invoiceItemType)
            : this(invoice, invoiceItemType, null)
        {}

        public InvoiceItem (IInvoice invoice, InvoiceItemType invoiceItemType, int? parentId)  
            : this(invoice, EnumTypeFieldConverter.InvoiceItem.GetTypeField(invoiceItemType).TypeKey, parentId)
        {}

        public InvoiceItem(IInvoice invoice, Guid invoiceItemTfKey, int? parentId)
            : this(invoice.Id, invoiceItemTfKey, parentId)
        {}

        internal InvoiceItem(int invoiceId, Guid invoiceItemTfKey, int? parentId)
        {
            _invoiceId = invoiceId;
            _invoiceItemTfKey = invoiceItemTfKey;
            _parentId = parentId;
        }
        
        private static readonly PropertyInfo InvoiceItemTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<InvoiceItem, Guid>(x => x.InvoiceItemTfKey);  
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<InvoiceItem, string>(x => x.Sku);  
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<InvoiceItem, string>(x => x.Name);  
        private static readonly PropertyInfo QuantitySelector = ExpressionHelper.GetPropertyInfo<InvoiceItem, int>(x => x.Quantity);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<InvoiceItem, decimal>(x => x.Amount);  
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<InvoiceItem, bool>(x => x.Exported);  
        
        /// <summary>
        /// The parentId associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public int? ParentId
        {
            get { return _parentId; }
        }
    
        /// <summary>
        /// The invoiceId associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public int InvoiceId
        {
            get { return _invoiceId; }
        }
    
        /// <summary>
        /// The invoiceItemTypeFieldKey associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public Guid InvoiceItemTfKey
        {
            get { return _invoiceItemTfKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceItemTfKey = value;
                    return _invoiceItemTfKey;
                }, _invoiceItemTfKey, InvoiceItemTypeFieldKeySelector); 
            }
        }
    
        /// <summary>
        /// The sku associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public string Sku
        {
            get { return _sku; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _sku = value;
                        return _sku;
                    }, _sku, SkuSelector); 
                }
        }
    
        /// <summary>
        /// The name associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _name = value;
                        return _name;
                    }, _name, NameSelector); 
                }
        }
    
        /// <summary>
        /// The baseQuantity associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public int Quantity
        {
            get { return _quantity; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _quantity = value;
                        return _quantity;
                    }, _quantity, QuantitySelector); 
                }
        }
    
        /// <summary>
        /// The amount associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _amount = value;
                        return _amount;
                    }, _amount, AmountSelector); 
                }
        }
    
        /// <summary>
        /// The exported associated with the InvoiceItem
        /// </summary>
        [DataMember]
        public bool Exported
        {
            get { return _exported; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _exported = value;
                        return _exported;
                    }, _exported, ExportedSelector); 
                }
        }

        [DataMember]
        public InvoiceItemType InvoiceItemType
        {
            get { return EnumTypeFieldConverter.InvoiceItem.GetTypeField(_invoiceItemTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.InvoiceItem.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    InvoiceItemTfKey = reference.TypeKey;
                }
            }
        }
        
    }

}