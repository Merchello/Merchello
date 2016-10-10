namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a province from a taxation context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class TaxProvince : Province, ITaxProvince
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxProvince"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public TaxProvince(string code, string name) 
            : base(code, name)
        {
            PercentAdjustment = 0M;
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal PercentAdjustment { get; set; }
    }
}