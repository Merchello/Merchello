namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal sealed class AddressTypeField : ExtendedTypeFieldMapper<AddressType>, IAddressTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressTypeField"/> class.
        /// </summary>
        internal AddressTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Shipping
        {
            get
            {
                return GetTypeField(AddressType.Shipping);
            }
        }

        /// <inheritdoc/>
        public ITypeField Billing
        {
            get
            {
                return GetTypeField(AddressType.Billing);
            }
        }

        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(AddressType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.Address.ShippingAddressKey));
            AddUpdateCache(AddressType.Billing,  new TypeField("Billing", "Billing", Constants.TypeFieldKeys.Address.BillingAddressKey));
            AddUpdateCache(AddressType.Custom, NotFound);
        }        
    }
}
