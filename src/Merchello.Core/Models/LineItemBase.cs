namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;

    using Merchello.Core.Logging;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Represents a line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class LineItemBase : Entity, ILineItem
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

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

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
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
        /// The line item type field key.
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
                MultiLogHelper.Debug<LineItemBase>("LineItemType.Custom cannot be added to a collection.  You need to pass the type field key directly.");
            }

            Ensure.ParameterCondition(lineItemTfKey != Guid.Empty, "lineItemTfKey");
            Ensure.ParameterNotNull(extendedData, "extendedData");
            Ensure.ParameterNotNullOrEmpty(name, "name");
            Ensure.ParameterNotNullOrEmpty(sku, "sku");
                        
            _lineItemTfKey = lineItemTfKey;
            _name = name;
            _sku = sku;
            _quantity = quantity;
            _price = price;
            _extendedData = extendedData;
        }


        /// <inheritdoc/>
        [DataMember]
        public Guid ContainerKey
        {
            get
            {
                return _containerKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _containerKey, _ps.Value.ContainerKeySelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid LineItemTfKey
        {
            get
            {
                return _lineItemTfKey;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _lineItemTfKey, _ps.Value.LineItemTfKeySelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Sku
        {
            get
            {
                return _sku;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _sku, _ps.Value.SkuSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _quantity, _ps.Value.QuantitySelector); 
            }
        }
    
        /// <inheritdoc/>
        [DataMember]
        public decimal Price
        {
            get
            {
                return _price;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _price, _ps.Value.PriceSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            { 
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public LineItemType LineItemType
        {
            get
            {
                return EnumTypeFieldConverter.LineItemType.GetTypeField(_lineItemTfKey);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public decimal TotalPrice 
        {
            get
            {
                return _price * _quantity;
            }
        }


        /// <inheritdoc/>
        [DataMember]
        public bool Exported
        {
            get
            {
                return _exported;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _exported, _ps.Value.ExportedSelector);
            }
        }

        /// <inheritdoc/>
        public virtual void Accept(ILineItemVisitor vistor)
        {
            vistor.Visit(this);
        }

        /// <summary>
        /// Serializes the current instance to an Xml representation - intended to be persisted in an <see cref="ExtendedDataCollection"/>  
        /// </summary>
        /// <returns>An Xml string</returns>
        [Obsolete("TODO this should serialize to JSON and be an extension")]
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

        /// <summary>
        /// Handles the extended data collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The container key selector.
            /// </summary>
            public readonly PropertyInfo ContainerKeySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, Guid>(x => x.ContainerKey);

            /// <summary>
            /// The line item type field key selector.
            /// </summary>
            public readonly PropertyInfo LineItemTfKeySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, Guid>(x => x.LineItemTfKey);

            /// <summary>
            /// The SKU selector.
            /// </summary>
            public readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Sku);

            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, string>(x => x.Name);

            /// <summary>
            /// The quantity selector.
            /// </summary>
            public readonly PropertyInfo QuantitySelector = ExpressionHelper.GetPropertyInfo<LineItemBase, int>(x => x.Quantity);

            /// <summary>
            /// The price selector.
            /// </summary>
            public readonly PropertyInfo PriceSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, decimal>(x => x.Price);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);

            /// <summary>
            /// The exported selector.
            /// </summary>
            public readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, bool>(x => x.Exported);
        }
    }
}