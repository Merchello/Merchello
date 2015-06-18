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
    /// Represents marketing Offer Settings.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class OfferSettings : Entity, IOfferSettings
    {
        #region Fields

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, string>(x => x.Name);

        /// <summary>
        /// The offer code selector.
        /// </summary>
        private static readonly PropertyInfo OfferCodeSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, string>(x => x.OfferCode);

        /// <summary>
        /// The offer provider key selector.
        /// </summary>
        private static readonly PropertyInfo OfferProviderKeySelector = ExpressionHelper.GetPropertyInfo<OfferSettings, Guid>(x => x.OfferProviderKey);

        /// <summary>
        /// The offer starts date selector.
        /// </summary>
        private static readonly PropertyInfo OfferStartsDateSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, DateTime>(x => x.OfferStartsDate);

        /// <summary>
        /// The offer ends date selector.
        /// </summary>
        private static readonly PropertyInfo OfferEndsDateSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, DateTime>(x => x.OfferEndsDate);

        /// <summary>
        /// The active selector.
        /// </summary>
        private static readonly PropertyInfo ActiveSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, bool>(x => x.Active);

        /// <summary>
        /// The component configurations changed selector.
        /// </summary>
        private static readonly PropertyInfo ComponentDefinitionsChangedSelector = ExpressionHelper.GetPropertyInfo<OfferSettings, OfferComponentDefinitionCollection>(x => x.ComponentDefinitions);

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
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(offerCode, "offerCode");
            Mandate.ParameterCondition(!Guid.Empty.Equals(offerProviderKey), "offerProviderKey");
            Mandate.ParameterNotNull(componentDefinitions, "ComponentDefinitions");
            _name = name;
            _offerCode = offerCode;
            _offerProviderKey = offerProviderKey;
            _offerStartsDate = DateTime.MinValue;
            _offerEndsDate = DateTime.MaxValue;
            _active = true;
            this._componentDefinitions = componentDefinitions;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _name = value;
                        return _name;
                    }, 
                    _name, 
                    NameSelector);
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
        /// Gets or sets the offer starts date.
        /// </summary>
        [DataMember]
        public DateTime OfferStartsDate
        {
            get
            {
                return _offerStartsDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _offerStartsDate = value;
                        return _offerStartsDate;
                    },
                    _offerStartsDate,
                    OfferStartsDateSelector);
            }                
        }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        [DataMember]
        public DateTime OfferEndsDate
        {
            get
            {
                return _offerEndsDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _offerEndsDate = value;
                        return _offerEndsDate;
                    },
                    _offerEndsDate,
                    OfferEndsDateSelector);
            }      
        }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [DataMember]
        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _active = value;
                        return _active;
                    },
                    _active,
                    ActiveSelector);
            }      
        }

        /// <summary>
        /// Gets a value indicating whether the offer has expired.
        /// </summary>
        [DataMember]
        public bool Expired
        {
            get
            {
                return !(_offerStartsDate == DateTime.MinValue && _offerEndsDate == DateTime.MaxValue) && 
                    DateTime.Now > _offerEndsDate.AddDays(1); // add one to the days since we are using midnight
            }
        }

        /// <summary>
        /// Gets a value indicating whether has started.
        /// </summary>
        [DataMember]
        public bool HasStarted
        {
            get
            {
                return DateTime.Now >= _offerStartsDate;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="OfferComponentDefinitionCollection"/>.
        /// </summary>
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
        /// Event handler for the <see cref="OfferComponentDefinitionCollection"/>.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="notifyCollectionChangedEventArgs">
        /// The notify collection changed event args.
        /// </param>
        private void ComponentDefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnPropertyChanged(ComponentDefinitionsChangedSelector);
        }
    }
}