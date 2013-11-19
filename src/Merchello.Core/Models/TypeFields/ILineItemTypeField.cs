namespace Merchello.Core.Models.TypeFields
{
    public interface ILineItemTypeField : ITypeFieldMapper<LineItemType>
    {
        /// <summary>
        /// Represents a product line item type
        /// </summary>
        ITypeField Product { get; }

        /// <summary>
        /// Represents a shipping line item type
        /// </summary>
        ITypeField Shipping { get; }

        /// <summary>
        /// Represents a tax line item type
        /// </summary>
        ITypeField Tax { get; }

        /// <summary>
        /// Represents a discount line item type
        /// </summary>
        ITypeField Discount { get;  }

    }
}