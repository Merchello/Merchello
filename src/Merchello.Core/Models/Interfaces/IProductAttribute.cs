using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product attribute
    /// </summary>
    public interface IProductAttribute : ISimpleEntity
    {

        /// <summary>
        /// The id of the option which defines the attribute group this attribute belongs to
        /// </summary>
        [DataMember]
        int OptionId { get; set;  }

        /// <summary>
        /// The name of the attribute
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The suggested sku concatenation
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// The sort order for the product attribute with respect to an option
        /// </summary>
        [DataMember]
        int SortOrder { get; set; }
    }
}