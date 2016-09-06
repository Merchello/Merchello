namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Defines an AddressTypeField
    /// </summary>
    public interface IAddressTypeField : ITypeFieldMapper<AddressType>
    {
        /// <summary>
        /// Gets the residential type
        /// </summary>
        ITypeField Shipping { get; }     

        /// <summary>
        /// Gets the commercial type
        /// </summary>
        ITypeField Billing { get; }
    }
}