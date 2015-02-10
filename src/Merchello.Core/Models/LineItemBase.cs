namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Represents a line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class LineItemBase : Entity, ILineItem
    {
        /// <summary>
        /// The container key.
        /// </summary>
        private Guid _containerKey;

        /// <summary>
        /// The line item TypeField key.
        /// </summary>
        private Guid _lineItemTfKey;

        /// <summary>
        /// The SKU.
        /// </summary>
        private string _sku;

        /// <summary>
        /// The _name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The Quantity.
        /// </summary>
        private int _quantity;

        /// <summary>
        /// The Price.
        /// </summary>
        private decimal _price;

        /// <summary>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        /// <summary>
        /// The exported.
        /// </summary>
        private bool _exported;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemBase"/> class.
        /// </summary>
        internal LineItemBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemBase"/> class.
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
        protected LineItemBase(string name, string sku, decimal amount)
            : this(name, sku, 1, amount)
        {
        }

        protected LineItemBase(string name, string sku, int quantity, decimal amount)
            : this(LineItemType.Product, name, sku, quantity, amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemBase"/> class.
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
        protected LineItemBase(LineItemType lineItemType, string name, string sku, int quantity, decimal price)
            : this(
                EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey,
                name,
                sku,
                quantity,
                price,
                new ExtendedDataCollection())
        {  
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemBase"/> class.
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
        protected LineItemBase(
            LineItemType lineItemType,
            string name,
            string sku,
            int quantity,
            decimal price,
            ExtendedDataCollection extendedData)
            : this(
                EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey,
                name,
                sku,
                quantity,
                price,
                extendedData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemBase"/> class.
        /// </summary>
        /// <param name="lineItemTfKey">
        /// The line item tf key.
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
        protected LineItemBase(Guid lineItemTfKey, string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData)  
        {
            if (lineItemTfKey.Equals(Guid.Empty))
            {
                // This could be a custom type field
                LogHelper.Debug<LineItemBase>("LineItemType.Custom cannot be added to a collection.  You need to pass the type field key directly.");
            }

            Mandate.ParameterCondition(lineItemTfKey != Guid.Empty, "lineItemTfKey");
            Mandate.ParameterNotNull(extendedData, "extendedData");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
                        
            _lineItemTfKey = lineItemTfKey;
            _name = name;
            _sku = sku;
            _quantity = quantity;
            _price = price;
            _extendedData = extendedData;
        }

        private static readonly PropertyInfo ContainerKeySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, Guid>(x => x.ContainerKey);
        private static readonly PropertyInfo LineItemTfKeySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, Guid>(x => x.LineItemTfKey);
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Sku);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Name);
        private static readonly PropertyInfo BaseQuantitySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, int>(x => x.Quantity);
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, decimal>(x => x.Price);
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, bool>(x => x.Exported);

        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }


        /// <summary>
        /// The "container" or parent of collection's primary 'key' (Guid)
        /// </summary>
        [DataMember]
        public Guid ContainerKey
        {
            get { return _containerKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _containerKey = value;
                    return _containerKey;
                }, _containerKey, ContainerKeySelector); 
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
        public decimal Price
        {
            get { return _price; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _price = value;
                        return _price;
                    }, _price, AmountSelector); 
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
        /// The total price of the line item (quantity * price)
        /// </summary>
        [IgnoreDataMember]
        public decimal TotalPrice 
        { 
            get { return _price *_quantity; }
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
                    writer.WriteElementString(Constants.ExtendedDataKeys.Price, Price.ToString(CultureInfo.InvariantCulture));
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