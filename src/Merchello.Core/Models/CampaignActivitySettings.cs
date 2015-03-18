namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;

    /// <summary>
    /// The campaign activity base.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CampaignActivitySettings : CampaignBase, ICampaignActivitySettings
    {
        #region Fields

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<CampaignActivitySettings, ExtendedDataCollection>(x => x.ExtendedData);

        /// <summary>
        /// The start date selector.
        /// </summary>
        private static readonly PropertyInfo StartDateSelector = ExpressionHelper.GetPropertyInfo<CampaignActivitySettings, DateTime>(x => x.StartDate);

        /// <summary>
        /// The end date selector.
        /// </summary>
        private static readonly PropertyInfo EndDateSelector = ExpressionHelper.GetPropertyInfo<CampaignActivitySettings, DateTime>(x => x.EndDate);

        /// <summary>
        /// The start date.
        /// </summary>
        private DateTime _startDate;

        /// <summary>
        /// The end date.
        /// </summary>
        private DateTime _endDate;

        /// <summary>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </summary>
        private ExtendedDataCollection _extendedData;
        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivitySettings"/> class.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <param name="campaignActivityTfKey">
        /// The campaign activity type field key.
        /// </param>
        internal CampaignActivitySettings(Guid campaignKey, Guid campaignActivityTfKey)
        {
            Mandate.ParameterCondition(!campaignKey.Equals(Guid.Empty), "campaignKey");
            Mandate.ParameterCondition(!campaignActivityTfKey.Equals(Guid.Empty), "campaignActivityTfKey");

            this.CampaignKey = campaignKey;
            CampaignActivityTfKey = campaignActivityTfKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivitySettings"/> class.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <param name="campaignActivityType">
        /// The campaign activity type.
        /// </param>
        internal CampaignActivitySettings(Guid campaignKey, CampaignActivityType campaignActivityType)
        {
            Mandate.ParameterCondition(!campaignKey.Equals(Guid.Empty), "campaignKey");
            Mandate.ParameterCondition(!campaignActivityType.Equals(CampaignActivityType.Custom), "campaignActivityType");

            CampaignKey = campaignKey;

            CampaignKey = EnumTypeFieldConverter.CampaignActivity.GetTypeField(campaignActivityType).TypeKey;
        }

        /// <summary>
        /// Gets the campaign key.
        /// </summary>
        [DataMember]
        public Guid CampaignKey
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets the campaign activity type field key.
        /// </summary>
        public Guid CampaignActivityTfKey
        {
            get; 
            
            private set; 
        }

        /// <summary>
        /// Gets the campaign activity type.
        /// </summary>
        public CampaignActivityType CampaignActivityType
        {
            get
            {
                return EnumTypeFieldConverter.CampaignActivity.GetTypeField(CampaignActivityTfKey);
            }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [DataMember]
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _startDate = value;
                        return _startDate;
                    },
                _startDate,
                StartDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [DataMember]
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _endDate = value;
                        return _endDate;
                    },
                _endDate,
                EndDateSelector);
            }
        }

        /// <summary>
        /// Gets the extended data.
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
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args e
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }
}