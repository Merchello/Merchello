namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents an ItemCacheTypeField
    /// </summary>
    public interface IItemCacheTypeField : IExtendedTypeFieldMapper<ItemCacheType>
    {
        /// <summary>
        /// Gets the basket <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Basket { get; }

        /// <summary>
        /// Gets the wish list <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Wishlist { get; }

        /// <summary>
        /// Gets the checkout <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Checkout { get; }
    }
}