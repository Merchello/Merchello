﻿using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class LineItemBase : IdEntity, ILineItem
    {
        private int _containerId;     
        private Guid _lineItemTfKey;
        private string _sku;
        private string _name;
        private int _quantity;
        private decimal _amount;
        private ExtendedDataCollection _extendedData;
        private bool _exported;

        protected LineItemBase(int containerId, string name, string sku, decimal amount)
            : this(containerId, name, sku, 1, amount)
        { }

        protected LineItemBase(int containerId, string name, string sku, int quantity, decimal amount)
            : this(containerId, LineItemType.Product, name, sku, quantity, amount)
        { }

        protected LineItemBase(int containerId, LineItemType lineItemType, string name, string sku, int quantity, decimal amount)
            : this(containerId, EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey, name, sku, quantity, amount, new ExtendedDataCollection())
        { }

        protected LineItemBase(int containerId, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData)
            : this(containerId, EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey, name, sku, quantity, amount, extendedData)
        { }

        protected LineItemBase(int containerId, Guid lineItemTfKey, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData)  
        {
            Mandate.ParameterCondition(containerId != 0, "containerId");
            Mandate.ParameterCondition(lineItemTfKey != Guid.Empty, "lineItemTfKey");
            Mandate.ParameterNotNull(extendedData, "extendedData");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
            
            _containerId = containerId;
            _lineItemTfKey = lineItemTfKey;
            _name = name;
            _sku = sku;
            _quantity = quantity;
            _amount = amount;
            _extendedData = extendedData;
        }

        private static readonly PropertyInfo LineItemTfKeySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, Guid>(x => x.LineItemTfKey);
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Sku);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Name);
        private static readonly PropertyInfo BaseQuantitySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, int>(x => x.Quantity);
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, decimal>(x => x.Amount);
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, bool>(x => x.Exported);

        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }


        /// <summary>
        /// The customer registry id associated with the Customer Registry
        /// </summary>
        [DataMember]
        public int ContainerId
        {
            get { return _containerId; }
            internal set
            {
                _containerId = value;
            }
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

        /// <summary>
        /// A collection for storing custom/extended line item data
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get { return _extendedData; }
            internal set { 
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// True/false indicating whether or not this line item has been exported to an external system
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

        /// <summary>
        /// Accept for visitor operations
        /// </summary>
        /// <param name="vistor"><see cref="ILineItemVisitor"/></param>
        public virtual void Accept(ILineItemVisitor vistor)
        {
            vistor.Visit(this);
        }
    }
}