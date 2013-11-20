using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    public interface IProvince
    {
        /// <summary>
        /// The name of the province
        /// </summary>
        [DataMember]
        string Name { get; }

        /// <summary>
        /// The two letter province code
        /// </summary>
        [DataMember]
        string Code { get; }

        /// <summary>
        /// True/false indicating whether or not to allow shipping to the province
        /// </summary>
        [DataMember]
        bool ShipTo { get; set; }

        /// <summary>
        /// Price adjustment when shipping to this province
        /// </summary>
        [DataMember]
        decimal ShippingAdjustment { get; set; }

        /// <summary>
        /// Tax percentage adjustment for orders shipped to this province
        /// </summary>
        [DataMember]
        decimal TaxPercentAdjustment { get; set; }
    }
}