namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a province from a shipping context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ShipProvince : Province, IShipProvince
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipProvince"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public ShipProvince(string code, string name)
            : base(code, name)
        {
            AllowShipping = true;
            RateAdjustment = 0;
            RateAdjustmentType = RateAdjustmentType.Numeric;
        }

        /// <inheritdoc/>
        [DataMember]
        public bool AllowShipping { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public decimal RateAdjustment { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public RateAdjustmentType RateAdjustmentType { get; set; }
    }
}