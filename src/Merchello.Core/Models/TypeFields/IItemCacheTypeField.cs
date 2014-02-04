namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines a BasketTypeField
    /// </summary>
    public interface IItemCacheTypeField : ITypeFieldMapper<ItemCacheType>
    {
     
        /// <summary>
        /// The basket type
        /// </summary>
        ITypeField Basket { get; }

        /// <summary>
        /// The wishlist type
        /// </summary>
        ITypeField Wishlist { get; }

        /// <summary>
        /// The checkout type
        /// </summary>
        ITypeField Checkout { get; }
   
    }
}