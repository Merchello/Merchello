namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a ShipRateTier - used in flat rate shipping rate tables
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ShipRateTier : Entity, IShipRateTier
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The ship method key.
        /// </summary>
        private readonly Guid _shipMethodKey;

        /// <summary>
        /// Low range value.
        /// </summary>
        private decimal _rangeLow;

        /// <summary>
        /// High range value.
        /// </summary>
        private decimal _rangeHigh;

        /// <summary>
        /// The rate.
        /// </summary>
        private decimal _rate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipRateTier"/> class.
        /// </summary>
        /// <param name="shipMethodKey">
        /// The ship method key.
        /// </param>
        public ShipRateTier(Guid shipMethodKey)
        {
            Ensure.ParameterCondition(shipMethodKey != Guid.Empty, "shipMethodKey");

            _shipMethodKey = shipMethodKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ShipMethodKey
        {
            get
            {
                return _shipMethodKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal RangeLow 
        {
            get
            {
                return _rangeLow;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _rangeLow, _ps.Value.RangeLowSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal RangeHigh
        {
            get
            {
                return _rangeHigh;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _rangeHigh, _ps.Value.RangeHighSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal Rate
        {
            get
            {
                return _rate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _rate, _ps.Value.RateSelector); 
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The range low selector.
            /// </summary>
            public readonly PropertyInfo RangeLowSelector = ExpressionHelper.GetPropertyInfo<ShipRateTier, decimal>(x => x.RangeLow);

            /// <summary>
            /// The range high selector.
            /// </summary>
            public readonly PropertyInfo RangeHighSelector = ExpressionHelper.GetPropertyInfo<ShipRateTier, decimal>(x => x.RangeHigh);

            /// <summary>
            /// The rate selector.
            /// </summary>
            public readonly PropertyInfo RateSelector = ExpressionHelper.GetPropertyInfo<ShipRateTier, decimal>(x => x.Rate);
        }
    }
}