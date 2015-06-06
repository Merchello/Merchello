namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// Represents an offer redemption record.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class OfferRedeemed : Entity, IOfferRedeemed
    {
        #region Fields

        /// <summary>
        /// The offer settings key selector.
        /// </summary>
        private static readonly PropertyInfo OfferSettingsKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid?>(x => x.OfferSettingsKey);

        /// <summary>
        /// The offer code selector.
        /// </summary>
        private static readonly PropertyInfo OfferCodeSelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, string>(x => x.OfferCode);

        /// <summary>
        /// The offer provider key selector.
        /// </summary>
        private static readonly PropertyInfo OfferProviderKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid>(x => x.OfferProviderKey);

        /// <summary>
        /// The customer key selector.
        /// </summary>
        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid?>(x => x.CustomerKey);

        /// <summary>
        /// The redeemed date selector.
        /// </summary>
        private static readonly PropertyInfo RedeemedDateSelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, DateTime>(x => x.RedeemedDate);

        /// <summary>
        /// The invoice key selector.
        /// </summary>
        private static readonly PropertyInfo InvoiceKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid>(x => x.InvoiceKey);

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, ExtendedDataCollection>(x => x.ExtendedData);
       
        /// <summary>
        /// The offer settings key.
        /// </summary>
        private Guid? _offerSettingsKey;

        /// <summary>
        /// The offer code.
        /// </summary>
        private string _offerCode;

        /// <summary>
        /// The offer provider key.
        /// </summary>
        private Guid _offerProviderKey;

        /// <summary>
        /// The customer key.
        /// </summary>
        private Guid? _customerKey;

        /// <summary>
        /// The redemption date.
        /// </summary>
        private DateTime _redeemedDate;

        /// <summary>
        /// The invoice key.
        /// </summary>
        private Guid _invoiceKey;

        /// <summary>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRedeemed"/> class.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="offerSettingsKey">
        /// The offer settings key.
        /// </param>
        public OfferRedeemed(string offerCode, Guid offerProviderKey, Guid invoiceKey, Guid? offerSettingsKey)
        {
            Mandate.ParameterNotNullOrEmpty(offerCode, "offerCode");
            offerProviderKey.ParameterNotEmptyGuid("offerProviderKey");
            invoiceKey.ParameterNotEmptyGuid("invoiceKey");

            _offerSettingsKey = offerSettingsKey;
            _offerProviderKey = offerProviderKey;
            _offerCode = offerCode;
            _invoiceKey = invoiceKey;
            _extendedData = new ExtendedDataCollection();
        }

        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        [DataMember]
        public Guid? OfferSettingsKey
        {
            get
            {
                return _offerSettingsKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _offerSettingsKey = value;
                    return _offerSettingsKey;
                }, 
                _offerSettingsKey, 
                OfferSettingsKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        [DataMember]
        public string OfferCode
        {
            get
            {
                return _offerCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _offerCode = value;
                        return _offerCode;
                    },
                _offerCode,
                OfferCodeSelector);
            }
        }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        [DataMember]
        public Guid OfferProviderKey
        {
            get
            {
                return _offerProviderKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _offerProviderKey = value;
                        return _offerProviderKey;
                    },
                _offerProviderKey,
                OfferProviderKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [DataMember]
        public Guid? CustomerKey
        {
            get
            {
                return _customerKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _customerKey = value;
                        return _customerKey;
                    },
                _customerKey,
                CustomerKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the redeemed date.
        /// </summary>
        [DataMember]
        public DateTime RedeemedDate
        {
            get
            {
                return _redeemedDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _redeemedDate = value;
                        return _redeemedDate;
                    },
                _redeemedDate,
                RedeemedDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        [DataMember]
        public Guid InvoiceKey
        {
            get
            {
                return _invoiceKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _invoiceKey = value;
                        return _invoiceKey;
                    },
                _invoiceKey,
                InvoiceKeySelector);
            }
        }

        /// <summary>
        /// Gets a collection for storing custom/extended line item data
        /// </summary>
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

        /// <summary>
        /// Overrides the AddingEntity method.
        /// </summary>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            RedeemedDate = DateTime.Now;
        }

        /// <summary>
        /// Event handled for the OnPropertyChanged event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }
}