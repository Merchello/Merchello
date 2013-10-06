using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping.Models
{
    public interface IOption : IIdEntity
    {
        /// <summary>
        /// The name of the option
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// True/false indicating whether or not it is required to select an option in order to purchase the associated product.
        /// </summary>
        /// <remarks>
        /// If true - a product item to product attribute relation is created defines the composition of a product item
        /// </remarks>
        [DataMember]
        bool Required { get; set; }

        /// <summary>
        /// The choices (product attributes) associated with this option
        /// </summary>
        [DataMember]
        ProductAttributeCollection Choices { get; set; }
    }
}