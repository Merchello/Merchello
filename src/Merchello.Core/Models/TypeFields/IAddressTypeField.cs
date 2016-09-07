namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents an AddressTypeField
    /// </summary>
    public interface IAddressTypeField : ITypeFieldMapper<AddressType>, ICanHaveCustomTypeFields
    {
        /// <summary>
        /// Gets the shipping <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Shipping { get; }

        /// <summary>
        /// Gets the billing  <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Billing { get; }
    }
}