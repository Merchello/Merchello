namespace Merchello.Core.Models
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a product option.
    /// </summary>
    public interface IProductOption : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the option
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the use name.
        /// </summary>
        string UseName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not it is required to select an option in order to purchase the associated product.
        /// </summary>
        /// <remarks>
        /// If true - a product item to product attribute relation is created defines the composition of a product item
        /// </remarks>
        bool Required { get; set; }

        /// <summary>
        /// Gets the order in which to list product option with respect to its product association.
        /// </summary>
        /// <remarks>
        /// This field is stored in the product 2 product option association and is not valid for shared option list (it is populated when associated with a product) - cache value should always be 0.
        /// </remarks>
        int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a shared option.
        /// </summary>
        bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        Guid? DetachedContentTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the UI option.
        /// </summary>
        string UiOption { get; set; }

        /// <summary>
        /// Gets or sets the choices (product attributes) associated with this option
        /// </summary>
        ProductAttributeCollection Choices { get; set; }

        /// <summary>
        /// Gets a clone version of the option.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        IProductOption Clone();
    }
}