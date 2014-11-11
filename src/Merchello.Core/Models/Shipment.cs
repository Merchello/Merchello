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
        #region Fields

        /// <summary>
        /// The shipment number prefix selector.
        /// </summary>
        private static readonly PropertyInfo ShipmentNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ShipmentNumberPrefix);

        /// <summary>
        /// The shipment number selector.
        /// </summary>
        private static readonly PropertyInfo ShipmentNumberSelector = ExpressionHelper.GetPropertyInfo<Shipment, int>(x => x.ShipmentNumber);

        /// <summary>
        /// The shipment status selector.
        /// </summary>
        private static readonly PropertyInfo ShipmentStatusSelector = ExpressionHelper.GetPropertyInfo<Shipment, IShipmentStatus>(x => x.ShipmentStatus);

        /// <summary>
        /// The shipped date selector.
        /// </summary>
        private static readonly PropertyInfo ShippedDateSelector = ExpressionHelper.GetPropertyInfo<Shipment, DateTime>(x => x.ShippedDate);

        /// <summary>
        /// The ship method key selector.
        /// </summary>
        private static readonly PropertyInfo ShipMethodKeySelector = ExpressionHelper.GetPropertyInfo<Shipment, Guid?>(x => x.ShipMethodKey);

        /// <summary>
        /// The from organization selector.
        /// </summary>
        private static readonly PropertyInfo FromOrganizationSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromOrganization);

        /// <summary>
        /// The from name selector.
        /// </summary>
        private static readonly PropertyInfo FromNameSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromName);

        /// <summary>
        /// The from address 1 selector.
        /// </summary>
        private static readonly PropertyInfo FromAddress1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromAddress1);

        /// <summary>
        /// The from address 2 selector.
        /// </summary>
        private static readonly PropertyInfo FromAddress2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromAddress2);

        /// <summary>
        /// The from locality selector.
        /// </summary>
        private static readonly PropertyInfo FromLocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromLocality);

        /// <summary>
        /// The from region selector.
        /// </summary>
        private static readonly PropertyInfo FromRegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromRegion);

        /// <summary>
        /// The from postal code selector.
        /// </summary>
        private static readonly PropertyInfo FromPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromPostalCode);

        /// <summary>
        /// The from country code selector.
        /// </summary>
        private static readonly PropertyInfo FromCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromCountryCode);

        /// <summary>
        /// The from is commercial selector.
        /// </summary>
        private static readonly PropertyInfo FromIsCommercialSelector = ExpressionHelper.GetPropertyInfo<Shipment, bool>(x => x.FromIsCommercial);

        /// <summary>
        /// The to organization selector.
        /// </summary>
        private static readonly PropertyInfo ToOrganizationSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToOrganization);

        /// <summary>
        /// The to name selector.
        /// </summary>
        private static readonly PropertyInfo ToNameSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToName);

        /// <summary>
        /// The to address 1 selector.
        /// </summary>
        private static readonly PropertyInfo ToAddress1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToAddress1);

        /// <summary>
        /// The to address 2 selector.
        /// </summary>
        private static readonly PropertyInfo ToAddress2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToAddress2);

        /// <summary>
        /// The to locality selector.
        /// </summary>
        private static readonly PropertyInfo ToLocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToLocality);

        /// <summary>
        /// The to region selector.
        /// </summary>
        private static readonly PropertyInfo ToRegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToRegion);

        /// <summary>
        /// The to postal code selector.
        /// </summary>
        private static readonly PropertyInfo ToPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToPostalCode);

        /// <summary>
        /// The to country code selector.
        /// </summary>
        private static readonly PropertyInfo ToCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToCountryCode);

        /// <summary>
        /// The to is commercial selector.
        /// </summary>
        private static readonly PropertyInfo ToIsCommercialSelector = ExpressionHelper.GetPropertyInfo<Shipment, bool>(x => x.ToIsCommercial);

        /// <summary>
        /// The phone selector.
        /// </summary>
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Phone);

        /// <summary>
        /// The email selector.
        /// </summary>
        private static readonly PropertyInfo EmailSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Email);

        /// <summary>
        /// The tracking code selector.
        /// </summary>
        private static readonly PropertyInfo TrackingCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.TrackingCode);

        /// <summary>
        /// The carrier selector.
        /// </summary>
        private static readonly PropertyInfo CarrierSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Carrier);

        private IShipmentStatus _shipmentStatus;
        private string _shipmentNumberPrefix;
        private int _shipmentNumber;
        private DateTime _shippedDate;
        private string _fromOrganization;
        private string _fromName;
        private string _fromAddress1;
        private string _fromAddress2;
        private string _fromLocality;
        private string _fromRegion;
        private string _fromPostalCode;
        private string _fromCountryCode;
        private bool _fromIsCommercial;
        private string _toOrganization;
        private string _toName;
        private string _toAddress1;
        private string _toAddress2;
        private string _toLocality;
        private string _toRegion;
        private string _toPostalCode;
        private string _toCountryCode;
        private bool _toIsCommercial;
        private Guid? _shipMethodKey;
        private string _email;
        private string _phone;
        private string _carrier;
        private string _trackingCode;
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
            Mandate.ParameterNotNull(shipmentStatus, "shipmentStatus");
            Mandate.ParameterNotNull(origin, "origin");
            Mandate.ParameterNotNull(destination, "destination");
            Mandate.ParameterNotNull(items, "items");

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

        /// <summary>
        /// Gets or sets the shipment number prefix.
        /// </summary>
        [DataMember]
        public string ShipmentNumberPrefix
        {
            get
            {
                return _shipmentNumberPrefix;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                   o =>
                   {
                       _shipmentNumberPrefix = value;
                       return _shipmentNumberPrefix;
                   },
               _shipmentNumberPrefix,
               ShipmentNumberPrefixSelector);
            }
        }

        /// <summary>
        /// Gets or sets the shipment number.
        /// </summary>
        [DataMember]
        public int ShipmentNumber
        {
            get
            {
                return _shipmentNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _shipmentNumber = value;
                        return _shipmentNumber;
                    },
                _shipmentNumber,
                ShipmentNumberSelector); 
            }
        }

        /// <summary>
        /// Gets the shipment status key.
        /// </summary>
        [DataMember]
        public Guid ShipmentStatusKey
        {
            get
            {
                return _shipmentStatus.Key;
            }  
        }

        /// <summary>
        /// Gets or sets the shipment status.
        /// </summary>
        [DataMember]
        public IShipmentStatus ShipmentStatus
        {
            get
            {
                return _shipmentStatus;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _shipmentStatus = value;
                        return _shipmentStatus;
                    },
                _shipmentStatus,
                ShipmentStatusSelector);
            }
        }

        /// <summary>
        /// Gets or sets the date the shipment was shipped
        /// </summary>
        [DataMember]
        public DateTime ShippedDate
        {
            get
            {
                return _shippedDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                        {
                    _shippedDate = value;
                    return _shippedDate;
                }, 
                _shippedDate, 
                ShippedDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        [DataMember]
        public string FromOrganization
        {
            get
            {
                return _fromOrganization;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromOrganization = value;
                    return _fromOrganization;
                }, 
                _fromOrganization, 
                FromOrganizationSelector);
            }
        }

        /// <summary>
        /// Gets or sets the name of origin address's name associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromName
        {
            get
            {
                return _fromName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromName = value;
                    return _fromName;
                }, 
                _fromName, 
                FromNameSelector);
            }
        }


        /// <summary>
        /// Gets or sets the first line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromAddress1
        {
            get
            {
                return _fromAddress1;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromAddress1 = value;
                    return _fromAddress1;
                }, 
                _fromAddress1, 
                FromAddress1Selector);
            }
        }

        /// <summary>
        /// Gets or sets the second line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromAddress2
        {
            get
            {
                return _fromAddress2;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromAddress2 = value;
                    return _fromAddress2;
                }, 
                _fromAddress2, 
                FromAddress2Selector);
            }
        }

        /// <summary>
        /// Gets or sets the locality or city of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromLocality
        {
            get
            {
                return _fromLocality;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromLocality = value;
                    return _toLocality;
                }, 
                _fromLocality, 
                FromLocalitySelector);
            }
        }

        /// <summary>
        /// Gets or sets the region, state, or province of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromRegion
        {
            get
            {
                return _fromRegion;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromRegion = value;
                    return _fromRegion;
                }, 
                _toRegion, 
                FromRegionSelector);
            }
        }

        /// <summary>
        /// Gets or sets the postal or zip code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromPostalCode
        {
            get
            {
                return _fromPostalCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromPostalCode = value;
                    return _fromPostalCode;
                }, 
                _fromPostalCode, 
                FromPostalCodeSelector);
            }
        }

        /// <summary>
        /// Gets or sets the country code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromCountryCode
        {
            get
            {
                return _fromCountryCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromCountryCode = value;
                    return _fromCountryCode;
                }, 
                _fromCountryCode, 
                FromCountryCodeSelector);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether or not the origin's address is a commercial address. Used by some shipping providers.
        /// </summary>
        [DataMember]
        public bool FromIsCommercial 
        {
            get
            {
                return _fromIsCommercial;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _fromIsCommercial = value;
                    return _fromIsCommercial;
                }, 
                _fromIsCommercial, 
                FromIsCommercialSelector);
            }
        }

        /// <summary>
        /// Gets or sets the organization or company name associated with the address
        /// </summary>
        [DataMember]
        public string ToOrganization 
        {
            get
            {
                return _toOrganization;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _toOrganization = value;
                    return _toOrganization;
                }, 
                _toOrganization, 
                ToOrganizationSelector);
            }
        }


        /// <summary>
        /// Gets or sets the name of destination address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToName
        {
            get
            {
                return _toName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _toName = value;
                    return _toName;
                }, 
                _toName, 
                ToNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the first line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToAddress1
        {
            get
            {
                return _toAddress1;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _toAddress1 = value;
                    return _toAddress1;
                }, 
                _toAddress1, 
                ToAddress1Selector); 
            }
        }
    
        /// <summary>
        /// Gets or sets the second line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToAddress2
        {
            get
            {
                return _toAddress2;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _toAddress2 = value;
                    return _toAddress2;
                }, 
                _toAddress2, 
                ToAddress2Selector); 
            }
        }
    
        /// <summary>
        /// The locality or city of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToLocality
        {
            get { return _toLocality; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _toLocality = value;
                        return _toLocality;
                    }, _toLocality, ToLocalitySelector); 
                }
        }
    
        /// <summary>
        /// The region, state, or province of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToRegion
        {
            get { return _toRegion; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _toRegion = value;
                        return _toRegion;
                    }, _toRegion, ToRegionSelector); 
                }
        }
    
        /// <summary>
        /// The postal or zip code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToPostalCode
        {
            get { return _toPostalCode; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _toPostalCode = value;
                        return _toPostalCode;
                    }, _toPostalCode, ToPostalCodeSelector); 
                }
        }
    
        /// <summary>
        /// The country code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToCountryCode
        {
            get { return _toCountryCode; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _toCountryCode = value;
                        return _toCountryCode;
                    }, _toCountryCode, ToCountryCodeSelector); 
                }
        }


        /// <summary>
        /// True/false indicating whether or not the destination address is a commercial address.  Used by some shipping providers.
        /// </summary>
        [DataMember]
        public bool ToIsCommercial 
        {
            get { return _toIsCommercial; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _toIsCommercial = value;
                    return _toIsCommercial;
                }, _toIsCommercial, ToIsCommercialSelector); 
            }
        }

        /// <summary>
        /// The ship method associated with this shipment
        /// </summary>
        /// <remarks>
        /// This is nullable in case a provider (and related shipmethods) is deleted and we want to maintain the shipment record
        /// </remarks>
        [DataMember]
        public Guid? ShipMethodKey
        {
            get { return _shipMethodKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _shipMethodKey = value;
                    return _shipMethodKey;
                }, _shipMethodKey, ShipMethodKeySelector);
            }
        }

        /// <summary>
        /// The phone at the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string Phone
        {
            get { return _phone; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _phone = value;
                    return _phone;
                }, _phone, PhoneSelector); 
            }
        }

        /// <summary>
        /// The contact email address associated with this shipment
        /// </summary>
        [DataMember]
        public string Email
        {
            get { return _email; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _email = value;
                    return _email;
                }, _email, EmailSelector);
            }
        }


        /// <summary>
        /// The name of the freight carrier associated with this shipment
        /// </summary>
        [DataMember]
        public string Carrier
        {
            get { return _carrier; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _carrier = value;
                    return _carrier;
                }, _carrier, CarrierSelector);
            }
        }

        /// <summary>
        /// The tracking code associated with this shipment
        /// </summary>
        [DataMember]
        public string TrackingCode
        {
            get { return _trackingCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _trackingCode = value;
                    return _trackingCode;
                }, _trackingCode, TrackingCodeSelector);
            }
        }

        /// <summary>
        /// The <see cref="ILineItem"/>s in the shipment
        /// </summary>
        [DataMember]
        public LineItemCollection Items
        {
            get
            {
                return _items;
            }
            internal set
            {
                _items = value;
            }
        }
    }

}