namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a product option.
    /// </summary>
    public interface IProductOption : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the option
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not it is required to select an option in order to purchase the associated product.
        /// </summary>
        /// <remarks>
        /// If true - a product item to product attribute relation is created defines the composition of a product item
        /// </remarks>
        [DataMember]
        bool Required { get; set; }

        /// <summary>
        /// Gets or sets the order in which to list product option with respect to its product association.
        /// </summary>
        /// <remarks>
        /// This field is stored in the product 2 product option association - cache value should always be null.
        /// </remarks>
        [DataMember]
        int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a shared option.
        /// </summary>
        [DataMember]
        bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the shared count - this is the number of products this option is associated with.
        /// </summary>
        [DataMember]
        int SharedCount { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDetachedContentType"/>.
        /// </summary>
        IDetachedContentType DetachedContentType { get; }

        /// <summary>
        /// Gets or sets the choices (product attributes) associated with this option
        /// </summary>
        [DataMember]
        ProductAttributeCollection Choices { get; set; }
    }
}