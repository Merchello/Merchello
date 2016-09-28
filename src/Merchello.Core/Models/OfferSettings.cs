namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents marketing Offer Settings.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class OfferSettings : Entity, IOfferSettings
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The offer name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The offer code.
        /// </summary>
        private string _offerCode;

        /// <summary>
        /// The offer provider key.
        /// </summary>
        private Guid _offerProviderKey;

        /// <summary>
        /// The offer starts date.
        /// </summary>
        private DateTime _offerStartsDate;

        /// <summary>
        /// The offer ends date.
        /// </summary>
        private DateTime _offerEndsDate;

        /// <summary>
        /// A value indicating whether or not the offer is active.
        /// </summary>
        private bool _active;

        /// <summary>
        /// The <see cref="OfferComponentDefinitionCollection"/>.
        /// </summary>
        private OfferComponentDefinitionCollection _componentDefinitions;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettings"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer Code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer Provider Key.
        /// </param>
        public OfferSettings(string name, string offerCode, Guid offerProviderKey)
            : this(name, offerCode, offerProviderKey, new OfferComponentDefinitionCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettings"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer Code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer Provider Key.
        /// </param>
        /// <param name="componentDefinitions">
        /// The <see cref="OfferComponentDefinitionCollection"/>.
        /// </param>
        internal OfferSettings(string name, string offerCode, Guid offerProviderKey, OfferComponentDefinitionCollection componentDefinitions)
        {
            Ensure.ParameterNotNullOrEmpty(name, "name");
            Ensure.ParameterNotNullOrEmpty(offerCode, "offerCode");
            Ensure.ParameterCondition(!Guid.Empty.Equals(offerProviderKey), "offerProviderKey");
            Ensure.ParameterNotNull(componentDefinitions, "ComponentDefinitions");
            _name = name;
            _offerCode = offerCode;
            _offerProviderKey = offerProviderKey;
            _offerStartsDate = DateTime.MinValue;
            _offerEndsDate = DateTime.MaxValue;
            _active = true;
            this._componentDefinitions = componentDefinitions;
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
        public DateTime OfferStartsDate
        {
            get
            {
                return _offerStartsDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _offerStartsDate, _ps.Value.OfferStartsDateSelector);
            }                
        }

        /// <inheritdoc/>
        [DataMember]
        public DateTime OfferEndsDate
        {
            get
            {
                return _offerEndsDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _offerEndsDate, _ps.Value.OfferEndsDateSelector);
            }      
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _active, _ps.Value.ActiveSelector);
            }      
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Expired
        {
            get
            {
                return !(_offerStartsDate == DateTime.MinValue && _offerEndsDate == DateTime.MaxValue) && 
                    DateTime.Now > _offerEndsDate.AddDays(1); // add one to the days since we are using midnight
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool HasStarted
        {
            get
            {
                return DateTime.Now >= _offerStartsDate;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public OfferComponentDefinitionCollection ComponentDefinitions
        {
            get
            {
                return this._componentDefinitions;
            }

            set
            {
                this._componentDefinitions = value;
                this._componentDefinitions.CollectionChanged += this.ComponentDefinitionsOnCollectionChanged;
            }
        }

        /// <summary>
        /// Handlers the <see cref="OfferComponentDefinitionCollection"/> changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="notifyCollectionChangedEventArgs">
        /// The notify collection changed event args.
        /// </param>
        private void ComponentDefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnPropertyChanged(_ps.Value.ComponentDefinitionsChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, string>(x => x.Name);

            /// <summary>
            /// The offer code selector.
            /// </summary>
            public readonly PropertyInfo OfferCodeSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, string>(x => x.OfferCode);

            /// <summary>
            /// The offer provider key selector.
            /// </summary>
            public readonly PropertyInfo OfferProviderKeySelector = ExpressionHelper.GetPropertyInfo<OfferSettings, Guid>(x => x.OfferProviderKey);

            /// <summary>
            /// The offer starts date selector.
            /// </summary>
            public readonly PropertyInfo OfferStartsDateSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, DateTime>(x => x.OfferStartsDate);

            /// <summary>
            /// The offer ends date selector.
            /// </summary>
            public readonly PropertyInfo OfferEndsDateSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, DateTime>(x => x.OfferEndsDate);

            /// <summary>
            /// The active selector.
            /// </summary>
            public readonly PropertyInfo ActiveSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, bool>(x => x.Active);

            /// <summary>
            /// The component configurations changed selector.
            /// </summary>
            public readonly PropertyInfo ComponentDefinitionsChangedSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, OfferComponentDefinitionCollection>(x => x.ComponentDefinitions);
        }
    }
}