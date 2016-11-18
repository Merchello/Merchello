namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a shipment.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [KnownType(typeof(LineItemCollection))]
    public class Shipment : VersionTaggedEntity, IShipment
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The shipment status.
        /// </summary>
        private IShipmentStatus _shipmentStatus;

        /// <summary>
        /// The shipment number prefix.
        /// </summary>
        private string _shipmentNumberPrefix;

        /// <summary>
        /// The shipment number.
        /// </summary>
        private int _shipmentNumber;

        /// <summary>
        /// The shipped date.
        /// </summary>
        private DateTime _shippedDate;

        /// <summary>
        /// From address organization.
        /// </summary>
        private string _fromOrganization;

        /// <summary>
        /// From address name.
        /// </summary>
        private string _fromName;

        /// <summary>
        /// From address 1.
        /// </summary>
        private string _fromAddress1;

        /// <summary>
        /// From address 2.
        /// </summary>
        private string _fromAddress2;

        /// <summary>
        /// From address locality.
        /// </summary>
        private string _fromLocality;

        /// <summary>
        /// From address region.
        /// </summary>
        private string _fromRegion;

        /// <summary>
        /// From address postal code.
        /// </summary>
        private string _fromPostalCode;

        /// <summary>
        /// From address country code.
        /// </summary>
        private string _fromCountryCode;

        /// <summary>
        /// From address is a commercial address.
        /// </summary>
        private bool _fromIsCommercial;

        /// <summary>
        /// To address organization.
        /// </summary>
        private string _toOrganization;

        /// <summary>
        /// To address name.
        /// </summary>
        private string _toName;

        /// <summary>
        /// To address 1.
        /// </summary>
        private string _toAddress1;

        /// <summary>
        /// To address 2.
        /// </summary>
        private string _toAddress2;

        /// <summary>
        /// To address locality.
        /// </summary>
        private string _toLocality;

        /// <summary>
        /// To address region.
        /// </summary>
        private string _toRegion;

        /// <summary>
        /// To address postal code.
        /// </summary>
        private string _toPostalCode;

        /// <summary>
        /// To address country code.
        /// </summary>
        private string _toCountryCode;

        /// <summary>
        /// To address is a commercial address.
        /// </summary>
        private bool _toIsCommercial;

        /// <summary>
        /// The ship method key.
        /// </summary>
        private Guid? _shipMethodKey;

        /// <summary>
        /// The email.
        /// </summary>
        private string _email;

        /// <summary>
        /// The phone.
        /// </summary>
        private string _phone;

        /// <summary>
        /// The carrier.
        /// </summary>
        private string _carrier;

        /// <summary>
        /// The tracking code.
        /// </summary>
        private string _trackingCode;

        /// <summary>
        /// The tracking url.
        /// </summary>
        private string _trackingUrl;

        /// <summary>
        /// The collection of items.
        /// </summary>
        private LineItemCollection _items;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Shipment"/> class.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment Status.
        /// </param>
        internal Shipment(IShipmentStatus shipmentStatus)
            : this(shipmentStatus, new Address(), new Address(), new LineItemCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shipment"/> class.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment Status.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        internal Shipment(IShipmentStatus shipmentStatus, IAddress origin, IAddress destination)
            : this(shipmentStatus, origin, destination, new LineItemCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shipment"/> class.
        /// </summary>
        /// <param name="shipmentStatus">
        /// The shipment Status.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        internal Shipment(IShipmentStatus shipmentStatus, IAddress origin, IAddress destination, LineItemCollection items)
        {
            Ensure.ParameterNotNull(shipmentStatus, "shipmentStatus");
            Ensure.ParameterNotNull(origin, "origin");
            Ensure.ParameterNotNull(destination, "destination");
            Ensure.ParameterNotNull(items, "items");

            _shippedDate = DateTime.Now;
            _fromOrganization = origin.Organization;
            _fromName = origin.Name;
            _fromAddress1 = origin.Address1;
            _fromAddress2 = origin.Address2;
            _fromLocality = origin.Locality;
            _fromRegion = origin.Region;
            _fromPostalCode = origin.PostalCode;
            _fromCountryCode = origin.CountryCode;
            _fromIsCommercial = origin.IsCommercial;
            _toOrganization = destination.Organization;
            _toName = destination.Name;
            _toAddress1 = destination.Address1;
            _toAddress2 = destination.Address2;
            _toLocality = destination.Locality;
            _toRegion = destination.Region;
            _toPostalCode = destination.PostalCode;
            _toCountryCode = destination.CountryCode;
            _toIsCommercial = destination.IsCommercial;

            _phone = destination.Phone;
            _email = destination.Email;

            _shipmentStatus = shipmentStatus;
            _items = items;
        }

        /// <inheritdoc/>
        [DataMember]
        public string ShipmentNumberPrefix
        {
            get
            {
                return _shipmentNumberPrefix;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shipmentNumberPrefix, _ps.Value.ShipmentNumberPrefixSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int ShipmentNumber
        {
            get
            {
                return _shipmentNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shipmentNumber, _ps.Value.ShipmentNumberSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ShipmentStatusKey
        {
            get
            {
                return _shipmentStatus.Key;
            }  
        }

        /// <inheritdoc/>
        [DataMember]
        public IShipmentStatus ShipmentStatus
        {
            get
            {
                return _shipmentStatus;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shipmentStatus, _ps.Value.ShipmentStatusSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public DateTime ShippedDate
        {
            get
            {
                return _shippedDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shippedDate, _ps.Value.ShippedDateSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromOrganization
        {
            get
            {
                return _fromOrganization;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromOrganization, _ps.Value.FromOrganizationSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromName
        {
            get
            {
                return _fromName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromName, _ps.Value.FromNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromAddress1
        {
            get
            {
                return _fromAddress1;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromAddress1, _ps.Value.FromAddress1Selector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromAddress2
        {
            get
            {
                return _fromAddress2;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromAddress2, _ps.Value.FromAddress2Selector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromLocality
        {
            get
            {
                return _fromLocality;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromLocality, _ps.Value.FromLocalitySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromRegion
        {
            get
            {
                return _fromRegion;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromRegion, _ps.Value.FromRegionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromPostalCode
        {
            get
            {
                return _fromPostalCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromPostalCode, _ps.Value.FromPostalCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string FromCountryCode
        {
            get
            {
                return _fromCountryCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromCountryCode, _ps.Value.FromCountryCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool FromIsCommercial 
        {
            get
            {
                return _fromIsCommercial;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _fromIsCommercial, _ps.Value.FromIsCommercialSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToOrganization 
        {
            get
            {
                return _toOrganization;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _toOrganization, _ps.Value.ToOrganizationSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToName
        {
            get
            {
                return _toName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _toName, _ps.Value.ToNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToAddress1
        {
            get
            {
                return _toAddress1;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _toAddress1, _ps.Value.ToAddress1Selector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToAddress2
        {
            get
            {
                return _toAddress2;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _toAddress2, _ps.Value.ToAddress2Selector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToLocality
        {
            get
            {
                return _toLocality;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _toLocality, _ps.Value.ToLocalitySelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToRegion
        {
            get
            {
                return _toRegion;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _toRegion, _ps.Value.ToRegionSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToPostalCode
        {
            get
            {
                return _toPostalCode;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _toPostalCode, _ps.Value.ToPostalCodeSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ToCountryCode
        {
            get
            {
                return _toCountryCode;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _toCountryCode, _ps.Value.ToCountryCodeSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool ToIsCommercial 
        {
            get
            {
                return _toIsCommercial;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _toIsCommercial, _ps.Value.ToIsCommercialSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? ShipMethodKey
        {
            get
            {
                return _shipMethodKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shipMethodKey, _ps.Value.ShipMethodKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Phone
        {
            get
            {
                return _phone;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _phone, _ps.Value.PhoneSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _email, _ps.Value.EmailSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Carrier
        {
            get
            {
                return _carrier;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _carrier, _ps.Value.CarrierSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string TrackingCode
        {
            get
            {
                return _trackingCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _trackingCode, _ps.Value.TrackingCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string TrackingUrl
        {
            get
            {
                return _trackingUrl;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _trackingUrl, _ps.Value.TrackingUrlSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public LineItemCollection Items
        {
            get
            {
                return _items;
            }

            internal 
                set
            {
                _items = value;
            }
        }

        /// <inheritdoc/>
        public void Accept(ILineItemVisitor visitor)
        {
            Items.Accept(visitor);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The shipment number prefix selector.
            /// </summary>
            public readonly PropertyInfo ShipmentNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ShipmentNumberPrefix);

            /// <summary>
            /// The shipment number selector.
            /// </summary>
            public readonly PropertyInfo ShipmentNumberSelector = ExpressionHelper.GetPropertyInfo<Shipment, int>(x => x.ShipmentNumber);

            /// <summary>
            /// The shipment status selector.
            /// </summary>
            public readonly PropertyInfo ShipmentStatusSelector = ExpressionHelper.GetPropertyInfo<Shipment, IShipmentStatus>(x => x.ShipmentStatus);

            /// <summary>
            /// The shipped date selector.
            /// </summary>
            public readonly PropertyInfo ShippedDateSelector = ExpressionHelper.GetPropertyInfo<Shipment, DateTime>(x => x.ShippedDate);

            /// <summary>
            /// The ship method key selector.
            /// </summary>
            public readonly PropertyInfo ShipMethodKeySelector = ExpressionHelper.GetPropertyInfo<Shipment, Guid?>(x => x.ShipMethodKey);

            /// <summary>
            /// The from organization selector.
            /// </summary>
            public readonly PropertyInfo FromOrganizationSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromOrganization);

            /// <summary>
            /// The from name selector.
            /// </summary>
            public readonly PropertyInfo FromNameSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromName);

            /// <summary>
            /// The from address 1 selector.
            /// </summary>
            public readonly PropertyInfo FromAddress1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromAddress1);

            /// <summary>
            /// The from address 2 selector.
            /// </summary>
            public readonly PropertyInfo FromAddress2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromAddress2);

            /// <summary>
            /// The from locality selector.
            /// </summary>
            public readonly PropertyInfo FromLocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromLocality);

            /// <summary>
            /// The from region selector.
            /// </summary>
            public readonly PropertyInfo FromRegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromRegion);

            /// <summary>
            /// The from postal code selector.
            /// </summary>
            public readonly PropertyInfo FromPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromPostalCode);

            /// <summary>
            /// The from country code selector.
            /// </summary>
            public readonly PropertyInfo FromCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromCountryCode);

            /// <summary>
            /// The from is commercial selector.
            /// </summary>
            public readonly PropertyInfo FromIsCommercialSelector = ExpressionHelper.GetPropertyInfo<Shipment, bool>(x => x.FromIsCommercial);

            /// <summary>
            /// The to organization selector.
            /// </summary>
            public readonly PropertyInfo ToOrganizationSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToOrganization);

            /// <summary>
            /// The to name selector.
            /// </summary>
            public readonly PropertyInfo ToNameSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToName);

            /// <summary>
            /// The to address 1 selector.
            /// </summary>
            public readonly PropertyInfo ToAddress1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToAddress1);

            /// <summary>
            /// The to address 2 selector.
            /// </summary>
            public readonly PropertyInfo ToAddress2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToAddress2);

            /// <summary>
            /// The to locality selector.
            /// </summary>
            public readonly PropertyInfo ToLocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToLocality);

            /// <summary>
            /// The to region selector.
            /// </summary>
            public readonly PropertyInfo ToRegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToRegion);

            /// <summary>
            /// The to postal code selector.
            /// </summary>
            public readonly PropertyInfo ToPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToPostalCode);

            /// <summary>
            /// The to country code selector.
            /// </summary>
            public readonly PropertyInfo ToCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToCountryCode);

            /// <summary>
            /// The to is commercial selector.
            /// </summary>
            public readonly PropertyInfo ToIsCommercialSelector = ExpressionHelper.GetPropertyInfo<Shipment, bool>(x => x.ToIsCommercial);

            /// <summary>
            /// The phone selector.
            /// </summary>
            public readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Phone);

            /// <summary>
            /// The email selector.
            /// </summary>
            public readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Email);

            /// <summary>
            /// The tracking code selector.
            /// </summary>
            public readonly PropertyInfo TrackingCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.TrackingCode);

            /// <summary>
            /// The tracking url selector.
            /// </summary>
            public readonly PropertyInfo TrackingUrlSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.TrackingUrl);

            /// <summary>
            /// The carrier selector.
            /// </summary>
            public readonly PropertyInfo CarrierSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Carrier);

        }
    }

}