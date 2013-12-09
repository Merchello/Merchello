using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    internal class Shipment : Entity, IShipment
    {
        private string _fromName;
        private string _fromAddress1;
        private string _fromAddress2;
        private string _fromLocality;
        private string _fromRegion;
        private string _fromPostalCode;
        private string _fromCountryCode;
        private string _toName;
        private string _toAddress1;
        private string _toAddress2;
        private string _toLocality;
        private string _toRegion;
        private string _toPostalCode;
        private string _toCountryCode;       
        private Guid? _shipMethodKey;
        private Guid? _invoiceItemKey;
        private string _phone;
       
        private static readonly PropertyInfo ShipMethodKeySelector = ExpressionHelper.GetPropertyInfo<Shipment, Guid?>(x => x.ShipMethodKey);
        private static readonly PropertyInfo InvoiceItemKeySelector = ExpressionHelper.GetPropertyInfo<Shipment, Guid?>(x => x.InvoiceItemKey);
        private static readonly PropertyInfo FromNameSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromName); 
        private static readonly PropertyInfo FromAddress1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromAddress1);
        private static readonly PropertyInfo FromAddress2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromAddress2);
        private static readonly PropertyInfo FromLocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromLocality);
        private static readonly PropertyInfo FromRegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromRegion);
        private static readonly PropertyInfo FromPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromPostalCode);
        private static readonly PropertyInfo FromCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.FromCountryCode);
        private static readonly PropertyInfo ToNameSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToName); 
        private static readonly PropertyInfo ToAddress1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToAddress1);  
        private static readonly PropertyInfo ToAddress2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToAddress2);  
        private static readonly PropertyInfo ToLocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToLocality);  
        private static readonly PropertyInfo ToRegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToRegion);  
        private static readonly PropertyInfo ToPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToPostalCode);  
        private static readonly PropertyInfo ToCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.ToCountryCode);         
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Phone);

        /// <summary>
        /// The name of origin address's name associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromName
        {
            get { return _fromName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromName = value;
                    return _fromName;
                }, _fromName, FromNameSelector);
            }
        }


        /// <summary>
        /// The first line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromAddress1
        {
            get { return _fromAddress1; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromAddress1 = value;
                    return _fromAddress1;
                }, _fromAddress1, FromAddress1Selector);
            }
        }

        /// <summary>
        /// The second line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromAddress2
        {
            get { return _fromAddress2; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromAddress2 = value;
                    return _fromAddress2;
                }, _fromAddress2, FromAddress2Selector);
            }
        }

        /// <summary>
        /// The locality or city of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromLocality
        {
            get { return _fromLocality; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromLocality = value;
                    return _toLocality;
                }, _fromLocality, FromLocalitySelector);
            }
        }

        /// <summary>
        /// The region, state, or province of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromRegion
        {
            get { return _fromRegion; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromRegion = value;
                    return _fromRegion;
                }, _toRegion, FromRegionSelector);
            }
        }

        /// <summary>
        /// The postal or zip code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromPostalCode
        {
            get { return _fromPostalCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromPostalCode = value;
                    return _fromPostalCode;
                }, _fromPostalCode, FromPostalCodeSelector);
            }
        }

        /// <summary>
        /// The country code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string FromCountryCode
        {
            get { return _fromCountryCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _fromCountryCode = value;
                    return _fromCountryCode;
                }, _fromCountryCode, FromCountryCodeSelector);
            }
        }


        /// <summary>
        /// The name of destination address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToName
        {
            get { return _toName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _toName = value;
                    return _toName;
                }, _toName, ToNameSelector);
            }
        }

        /// <summary>
        /// The first line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToAddress1
        {
            get { return _toAddress1; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _toAddress1 = value;
                        return _toAddress1;
                    }, _toAddress1, ToAddress1Selector); 
                }
        }
    
        /// <summary>
        /// The second line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string ToAddress2
        {
            get { return _toAddress2; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _toAddress2 = value;
                        return _toAddress2;
                    }, _toAddress2, ToAddress2Selector); 
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
        /// The invoice item, if any, associated with this shipment
        /// </summary>
        [DataMember]
        public Guid? InvoiceItemKey
        {
            get { return _invoiceItemKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceItemKey = value;
                    return _invoiceItemKey;
                }, _invoiceItemKey, InvoiceItemKeySelector);
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
                    
    }

}