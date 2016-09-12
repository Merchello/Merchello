namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents a LineItemTypeField.
    /// </summary>
    public interface ILineItemTypeField : IExtendedTypeFieldMapper<LineItemType>
    {
        /// <summary>
        /// Gets the product line item <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Product { get; }

        /// <summary>
        /// Gets the shipping line item <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Shipping { get; }

        /// <summary>
        /// Gets the tax line item <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Tax { get; }

        /// <summary>
        /// Gets the discount line item <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Discount { get;  }

        /// <summary>
        /// Gets the adjustment line item <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Adjustment { get; }
    }
}