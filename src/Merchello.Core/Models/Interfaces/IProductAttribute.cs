namespace Merchello.Core.Models
{
    using System;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a product attribute
    /// </summary>
    public interface IProductAttribute : IEntity
    {
        /// <summary>
        /// Gets or sets the key of the option which defines the attribute group this attribute belongs to
        /// </summary>
        
        Guid OptionKey { get; set;  }

        /// <summary>
        /// Gets or sets the name of the attribute
        /// </summary>
        
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the suggested SKU concatenation
        /// </summary>
        
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets sort order for the product attribute with respect to an option
        /// </summary>
        
        int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default choice.
        /// </summary>
        
        bool IsDefaultChoice { get; set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        
        //DetachedDataValuesCollection DetachedDataValues { get; }

        /// <summary>
        /// Returns a clone of the attribute.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductAttribute"/>.
        /// </returns>
        IProductAttribute Clone();
    }
}