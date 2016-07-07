namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a product attribute
    /// </summary>
    public interface IProductAttribute : IEntity
    {

        /// <summary>
        /// Gets or sets the key of the option which defines the attribute group this attribute belongs to
        /// </summary>
        [DataMember]
        Guid OptionKey { get; set;  }

        /// <summary>
        /// Gets or sets the name of the attribute
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the suggested SKU concatenation
        /// </summary>
        [DataMember]
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets sort order for the product attribute with respect to an option
        /// </summary>
        [DataMember]
        int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the use count - this is the number of product variants that use this attribute.
        /// </summary>
        [DataMember]
        int UseCount { get; set; }
    }
}