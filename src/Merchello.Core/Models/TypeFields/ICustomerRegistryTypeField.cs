namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines a BasketTypeField
    /// </summary>
    public interface ICustomerRegistryTypeField : ITypeFieldMapper<CustomerItemRegisterType>
    {
     
        /// <summary>
        /// The basket type
        /// </summary>
        ITypeField Basket { get; }

        /// <summary>
        /// The wishlist type
        /// </summary>
        ITypeField Wishlist { get; }
   
    }
}