namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Represents a line item type field
    /// </summary>
    public interface ILineItemTypeField : ITypeFieldMapper<LineItemType>
    {
        /// <summary>
        /// Gets the product line item type
        /// </summary>
        ITypeField Product { get; }

        /// <summary>
        /// Gets the shipping line item type
        /// </summary>
        ITypeField Shipping { get; }

        /// <summary>
        /// Gets the tax line item type
        /// </summary>
        ITypeField Tax { get; }

        /// <summary>
        /// Gets the discount line item type
        /// </summary>
        ITypeField Discount { get;  }

        /// <summary>
        /// Gets the adjustment line item type.
        /// </summary>
        ITypeField Adjustment { get; }
   
    }
}