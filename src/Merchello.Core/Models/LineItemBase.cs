using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class LineItemBase : IdEntity, ILineItem
    {
        private readonly int _containerId;     
        private Guid _lineItemTfKey;
        private string _sku;
        private string _name;
        private int _quantity;
        private decimal _amount;
        private LineItemCollection _itemization;

        protected LineItemBase (int containerId, Guid lineItemTfKey)
            : this(containerId, lineItemTfKey, new LineItemCollection())
        { }

        protected LineItemBase (int containerId, Guid lineItemTfKey, LineItemCollection itemization)  
        {
            Mandate.ParameterCondition(containerId != 0, "containerId");
            Mandate.ParameterCondition(lineItemTfKey != Guid.Empty, "lineItemTfKey");
            Mandate.ParameterNotNull(itemization, "itemization");

            _containerId = containerId;
            _itemization = itemization;
        }

        private static readonly PropertyInfo LineItemTfKeySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, Guid>(x => x.LineItemTfKey);
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Sku);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Name);
        private static readonly PropertyInfo BaseQuantitySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, int>(x => x.Quantity);
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, decimal>(x => x.Amount);

    
        /// <summary>
        /// The customer registry id associated with the Customer Registry
        /// </summary>
        [DataMember]
        public int ContainerId
        {
            get { return _containerId; }
        }
    
        /// <summary>
        /// The line item type field Key
        /// </summary>
        [DataMember]
        public Guid LineItemTfKey
        {
            get { return _lineItemTfKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _lineItemTfKey = value;
                    return _lineItemTfKey;
                }, _lineItemTfKey, LineItemTfKeySelector); 
            }
        }
    
        /// <summary>
        /// The sku associated with the line item in the customer registry
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
        /// The name or title of the item
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
        /// The quantity of items to be ordered
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
                    }, _quantity, BaseQuantitySelector); 
                }
        }
    
        /// <summary>
        /// The amount (cost) of the line item
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