using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    internal class BasketItem : IdEntity, IBasketItem
    {
        private readonly int _basketId;
        private int? _parentId;        
        private Guid _invoiceItemTypeFieldKey;
        private string _sku;
        private string _name;
        private int _baseQuantity;
        private int _unitOfMeasureMultiplier;
        private decimal _amount;

        public BasketItem (int basketId)  
        {
            _basketId = basketId;
        }
        
        private static readonly PropertyInfo ParentIdSelector = ExpressionHelper.GetPropertyInfo<BasketItem, int?>(x => x.ParentId);
        private static readonly PropertyInfo InvoiceItemTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<BasketItem, Guid>(x => x.InvoiceItemTypeFieldKey);  
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<BasketItem, string>(x => x.Sku);  
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<BasketItem, string>(x => x.Name);  
        private static readonly PropertyInfo BaseQuantitySelector = ExpressionHelper.GetPropertyInfo<BasketItem, int>(x => x.BaseQuantity);  
        private static readonly PropertyInfo UnitOfMeasureMultiplierSelector = ExpressionHelper.GetPropertyInfo<BasketItem, int>(x => x.UnitOfMeasureMultiplier);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<BasketItem, decimal>(x => x.Amount);  
        
        /// <summary>
        /// The parentId associated with the BasketItem
        /// </summary>
        [DataMember]
        public int? ParentId
        {
            get { return _parentId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _parentId = value;
                    return _parentId;
                }, _parentId, ParentIdSelector);
            }
        }
    
        /// <summary>
        /// The basketId associated with the BasketItem
        /// </summary>
        [DataMember]
        public int BasketId
        {
            get { return _basketId; }
        }
    
        /// <summary>
        /// The invoiceItemTypeFieldKey associated with the BasketItem
        /// </summary>
        [DataMember]
        public Guid InvoiceItemTypeFieldKey
        {
            get { return _invoiceItemTypeFieldKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceItemTypeFieldKey = value;
                    return _invoiceItemTypeFieldKey;
                }, _invoiceItemTypeFieldKey, InvoiceItemTypeFieldKeySelector); 
            }
        }
    
        /// <summary>
        /// The sku associated with the BasketItem
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
        /// The name associated with the BasketItem
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
        /// The baseQuantity associated with the BasketItem
        /// </summary>
        [DataMember]
        public int BaseQuantity
        {
            get { return _baseQuantity; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _baseQuantity = value;
                        return _baseQuantity;
                    }, _baseQuantity, BaseQuantitySelector); 
                }
        }
    
        /// <summary>
        /// The unitOfMeasureMultiplier associated with the BasketItem
        /// </summary>
        [DataMember]
        public int UnitOfMeasureMultiplier
        {
            get { return _unitOfMeasureMultiplier; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _unitOfMeasureMultiplier = value;
                        return _unitOfMeasureMultiplier;
                    }, _unitOfMeasureMultiplier, UnitOfMeasureMultiplierSelector); 
                }
        }
    
        /// <summary>
        /// The amount associated with the BasketItem
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
        
    }

}