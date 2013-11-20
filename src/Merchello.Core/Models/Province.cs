using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Province : IProvince
    {
        private readonly string _name;
        private readonly string _code;

        public Province(string code, string name)
        {
            _name = name;
            _code = code;
            ShipTo = true;
            ShippingAdjustment = 0;
            TaxPercentAdjustment = 0;
        }

        /// <summary>
        /// The name of the province
        /// </summary>
        [DataMember]
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// The two letter province code
        /// </summary>
        [DataMember]
        public string Code {
            get { return _code; }
        }

        /// <summary>
        /// True/false indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        public bool ShipTo { get; set; }

        /// <summary>
        /// Price adjustment when shipping to this province
        /// </summary>
        [DataMember]
        public decimal ShippingAdjustment { get; set; }

        /// <summary>
        /// Tax percentage adjustment for orders shipped to this province
        /// </summary>
        [DataMember]
        public decimal TaxPercentAdjustment { get; set; }
    }
}