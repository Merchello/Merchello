using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a province from a taxation context
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class TaxProvince : Province, ITaxProvince
    {
        public TaxProvince(string code, string name) 
            : base(code, name)
        {
            PercentAdjustment = 0M;
        }

        /// <summary>
        /// The percentage rate adjustment to the tax rate
        /// </summary>
        [DataMember]
        public decimal PercentAdjustment { get; set; }
    }
}