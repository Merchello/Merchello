using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class LineItemBase : Entity, ILineItem
    {
        private Guid _containerKey;     
        private Guid _lineItemTfKey;
        private string _sku;
        private string _name;
        private int _quantity;
        private decimal _amount;
        private ExtendedDataCollection _extendedData;
        private bool _exported;

        internal LineItemBase()
        {
        }

        protected LineItemBase(Guid containerKey, string name, string sku, decimal amount)
            : this(containerKey, name, sku, 1, amount)
        { }

        protected LineItemBase(Guid containerKey, string name, string sku, int quantity, decimal amount)
            : this(containerKey, LineItemType.Product, name, sku, quantity, amount)
        { }

        protected LineItemBase(Guid containerKey, LineItemType lineItemType, string name, string sku, int quantity, decimal amount)
            : this(containerKey, EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey, name, sku, quantity, amount, new ExtendedDataCollection())
        { }

        protected LineItemBase(Guid containerKey, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData)
            : this(containerKey, EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey, name, sku, quantity, amount, extendedData)
        { }

        protected LineItemBase(Guid containerKey, Guid lineItemTfKey, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData)  
        {
            Mandate.ParameterCondition(containerKey != Guid.Empty, "containerKey");
            Mandate.ParameterCondition(lineItemTfKey != Guid.Empty, "lineItemTfKey");
            Mandate.ParameterNotNull(extendedData, "extendedData");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
            
            _containerKey = containerKey;
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
        public Guid ContainerKey
        {
            get { return _containerKey; }
            internal set
            {
                _containerKey = value;
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
        /// Converts the LineItemTfKey to it's enum equivalent
        /// </summary>
        [DataMember]
        public LineItemType LineItemType
        {
            get
            {
                return EnumTypeFieldConverter.LineItemType.GetTypeField(_lineItemTfKey);
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

        /// <summary>
        /// Serializes the current instance to an Xml representation - intended to be persisted in an <see cref="ExtendedDataCollection"/>  
        /// </summary>
        /// <returns>An Xml string</returns>
        internal string SerializeToXml()
        {
            string xml;
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true
                    };

                using (var writer = XmlWriter.Create(sw,settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(Constants.ExtendedDataKeys.LineItem);


                    writer.WriteElementString(Constants.ExtendedDataKeys.ContainerKey, ContainerKey.ToString());
                    writer.WriteElementString(Constants.ExtendedDataKeys.LineItemTfKey, LineItemTfKey.ToString());
                    writer.WriteElementString(Constants.ExtendedDataKeys.Sku, Sku);
                    writer.WriteElementString(Constants.ExtendedDataKeys.Name, Name);
                    writer.WriteElementString(Constants.ExtendedDataKeys.Quantity, Quantity.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString(Constants.ExtendedDataKeys.Amount, Amount.ToString(CultureInfo.InvariantCulture));
                    writer.WriteStartElement(Constants.ExtendedDataKeys.ExtendedData);
                    writer.WriteRaw(ExtendedData.SerializeToXml());
                    writer.WriteEndElement();
                    writer.WriteEndElement(); // ExtendedData
                    writer.WriteEndDocument();
                    
                }
                xml = sw.ToString();
            }
            return xml;
        }
        
    }
}