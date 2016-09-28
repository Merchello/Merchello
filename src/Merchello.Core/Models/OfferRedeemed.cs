namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents an offer redemption record.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class OfferRedeemed : Entity, IOfferRedeemed
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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
            Ensure.ParameterNotNullOrEmpty(offerCode, "offerCode");
            offerProviderKey.ParameterNotEmptyGuid("offerProviderKey");
            invoiceKey.ParameterNotEmptyGuid("invoiceKey");

            _offerSettingsKey = offerSettingsKey;
            _offerProviderKey = offerProviderKey;
            _offerCode = offerCode;
            _invoiceKey = invoiceKey;
            _extendedData = new ExtendedDataCollection();
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? OfferSettingsKey
        {
            get
            {
                return _offerSettingsKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _offerSettingsKey, _ps.Value.OfferSettingsKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string OfferCode
        {
            get
            {
                return _offerCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _offerCode, _ps.Value.OfferCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid OfferProviderKey
        {
            get
            {
                return _offerProviderKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _offerProviderKey, _ps.Value.OfferProviderKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? CustomerKey
        {
            get
            {
                return _customerKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _customerKey, _ps.Value.CustomerKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public DateTime RedeemedDate
        {
            get
            {
                return _redeemedDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _redeemedDate, _ps.Value.RedeemedDateSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid InvoiceKey
        {
            get
            {
                return _invoiceKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _invoiceKey, _ps.Value.InvoiceKeySelector);
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
        internal override void AddingEntity()
        {
            base.AddingEntity();
            RedeemedDate = DateTime.Now;
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
            /// The offer settings key selector.
            /// </summary>
            public readonly PropertyInfo OfferSettingsKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid?>(x => x.OfferSettingsKey);

            /// <summary>
            /// The offer code selector.
            /// </summary>
            public readonly PropertyInfo OfferCodeSelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, string>(x => x.OfferCode);

            /// <summary>
            /// The offer provider key selector.
            /// </summary>
            public readonly PropertyInfo OfferProviderKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid>(x => x.OfferProviderKey);

            /// <summary>
            /// The customer key selector.
            /// </summary>
            public readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid?>(x => x.CustomerKey);

            /// <summary>
            /// The redeemed date selector.
            /// </summary>
            public readonly PropertyInfo RedeemedDateSelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, DateTime>(x => x.RedeemedDate);

            /// <summary>
            /// The invoice key selector.
            /// </summary>
            public readonly PropertyInfo InvoiceKeySelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, Guid>(x => x.InvoiceKey);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<OfferRedeemed, ExtendedDataCollection>(x => x.ExtendedData);
        }
    }
}