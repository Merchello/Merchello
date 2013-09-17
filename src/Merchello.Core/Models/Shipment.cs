using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Shipment : IdEntity, IShipment
    {
        private readonly int _invoiceId;
        private string _address1;
        private string _address2;
        private string _locality;
        private string _region;
        private string _postalCode;
        private string _countryCode;
        private int _shipMethodId;
        private string _phone;

        public Shipment (IInvoice invoice, IShipMethod shipMethod)  
            : this(invoice.Id, shipMethod.Id)
        { }

        internal Shipment(int invoiceId, int shipmethodId)
        {
            _invoiceId = invoiceId;
            _shipMethodId = shipmethodId;
        }
        
        private static readonly PropertyInfo ShipMethodSelector = ExpressionHelper.GetPropertyInfo<Shipment, int>(x => x.ShipMethodId);
        private static readonly PropertyInfo Address1Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Address1);  
        private static readonly PropertyInfo Address2Selector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Address2);  
        private static readonly PropertyInfo LocalitySelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Locality);  
        private static readonly PropertyInfo RegionSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Region);  
        private static readonly PropertyInfo PostalCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.PostalCode);  
        private static readonly PropertyInfo CountryCodeSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.CountryCode);  
        private static readonly PropertyInfo PhoneSelector = ExpressionHelper.GetPropertyInfo<Shipment, string>(x => x.Phone);  
        
        /// <summary>
        /// The invoiceId associated with the Shipment
        /// </summary>
        [DataMember]
        public int InvoiceId
        {
            get { return _invoiceId; }
        }
    
        /// <summary>
        /// The first line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string Address1
        {
            get { return _address1; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _address1 = value;
                        return _address1;
                    }, _address1, Address1Selector); 
                }
        }
    
        /// <summary>
        /// The second line of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string Address2
        {
            get { return _address2; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _address2 = value;
                        return _address2;
                    }, _address2, Address2Selector); 
                }
        }
    
        /// <summary>
        /// The locality or city of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string Locality
        {
            get { return _locality; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _locality = value;
                        return _locality;
                    }, _locality, LocalitySelector); 
                }
        }
    
        /// <summary>
        /// The region, state, or province of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string Region
        {
            get { return _region; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _region = value;
                        return _region;
                    }, _region, RegionSelector); 
                }
        }
    
        /// <summary>
        /// The postal or zip code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string PostalCode
        {
            get { return _postalCode; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _postalCode = value;
                        return _postalCode;
                    }, _postalCode, PostalCodeSelector); 
                }
        }
    
        /// <summary>
        /// The country code of the shipping address associated with the Shipment
        /// </summary>
        [DataMember]
        public string CountryCode
        {
            get { return _countryCode; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _countryCode = value;
                        return _countryCode;
                    }, _countryCode, CountryCodeSelector); 
                }
        }
    
        /// <summary>
        /// The shipMethodId associated with the Shipment
        /// </summary>
        [DataMember]
        public int ShipMethodId
        {
            get { return _shipMethodId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _shipMethodId = value;
                    return _shipMethodId;
                }, _shipMethodId, ShipMethodSelector);
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