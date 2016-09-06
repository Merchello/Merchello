namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Represents an item cache type field
    /// </summary>
    public interface IItemCacheTypeField : ITypeFieldMapper<ItemCacheType>
    {
     
        /// <summary>
        /// Gets the basket type field
        /// </summary>
        ITypeField Basket { get; }

        /// <summary>
        /// Gets the wish list type field
        /// </summary>
        ITypeField Wishlist { get; }

        /// <summary>
        /// Gets the checkout type field
        /// </summary>
        ITypeField Checkout { get; }
    }
}