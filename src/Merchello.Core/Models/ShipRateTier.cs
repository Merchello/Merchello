using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    using Umbraco.Core;

    /// <summary>
    /// Defines a ShipRateTier - used in flat rate shipping rate tables
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ShipRateTier : Entity, IShipRateTier
    {

        private readonly Guid _shipMethodKey;
        private decimal _rangeLow;
        private decimal _rangeHigh;
        private decimal _rate;

        public ShipRateTier(Guid shipMethodKey)
        {
            Mandate.ParameterCondition(shipMethodKey != Guid.Empty, "shipMethodKey");

            _shipMethodKey = shipMethodKey;
        }

        private static readonly PropertyInfo RangeLowSelector = ExpressionHelper.GetPropertyInfo<ShipRateTier, decimal>(x => x.RangeLow);
        private static readonly PropertyInfo RangeHighSelector = ExpressionHelper.GetPropertyInfo<ShipRateTier, decimal>(x => x.RangeHigh);
        private static readonly PropertyInfo RateSelector = ExpressionHelper.GetPropertyInfo<ShipRateTier, decimal>(x => x.Rate);

        /// <summary>
        /// The 'unique' key of the ship method
        /// </summary>
        [DataMember]
        public Guid ShipMethodKey {
            get { return _shipMethodKey; }
        }

        /// <summary>
        /// The low end of the range defined by this tier
        /// </summary>
        [DataMember]
        public decimal RangeLow 
        {
            get { return _rangeLow; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _rangeLow = value;
                    return _rangeLow;
                }, _rangeLow, RangeLowSelector); 
            }
        }

        /// <summary>
        /// The high end of the range defined by this tier
        /// </summary>
        [DataMember]
        public decimal RangeHigh
        {
            get { return _rangeHigh; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _rangeHigh = value;
                    return _rangeHigh;
                }, _rangeHigh, RangeHighSelector); 
            }

        }

        /// <summary>
        /// The rate or cost for this range
        /// </summary>
        [DataMember]
        public decimal Rate
        {
            get { return _rate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _rate = value;
                    return _rate;
                }, _rate, RateSelector); 
            }
        }
    }
}