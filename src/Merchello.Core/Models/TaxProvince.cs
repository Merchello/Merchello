using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.Interfaces;

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
            PercentAdjustment = 0;
        }

        /// <summary>
        /// Tax percentage adjustment for orders shipped to this province
        /// </summary>
        [DataMember]
        public decimal PercentAdjustment { get; set; }
    }
}