namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines an AddressTypeField
    /// </summary>
    public interface IAddressTypeField : ITypeFieldMapper<AddressType>
    {
        /// <summary>
        /// The residential type
        /// </summary>
        ITypeField Shipping { get; }     

        /// <summary>
        /// The commercial type
        /// </summary>
        ITypeField Billing { get; }
    }
}