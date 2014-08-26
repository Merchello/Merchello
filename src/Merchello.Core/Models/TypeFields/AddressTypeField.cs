using System;
using System.Linq;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Identifies an address as either shipping or billing
    /// </summary>
    internal sealed class AddressTypeField : TypeFieldMapper<AddressType>, IAddressTypeField
    {
        internal AddressTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

#region Overrides TypeFieldMapper<AddressType>


        internal override void BuildCache()
        {
            AddUpdateCache(AddressType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.Address.ShippingAddressKey));
            AddUpdateCache(AddressType.Billing,  new TypeField("Billing", "Billing", Constants.TypeFieldKeys.Address.BillingAddressKey));
        }

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return Addresses.GetTypeFields().Select(GetTypeField);
            }
        }

        #endregion

        /// <summary>
        /// Indicates the address is a residential address
        /// </summary>
        public ITypeField Shipping
        {
            get { return GetTypeField(AddressType.Shipping); }
        }

        /// <summary>
        /// Indicates the address is a billing address
        /// </summary>
        public ITypeField Billing
        {
            get { return GetTypeField(AddressType.Billing); }
        }

        /// <summary>
        /// Returns a custom address or NullTypeField TypeKey (Guid)
        /// </summary>
        /// <param name="alias">The alias of the custom address</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Addresses[alias]);
        }

        private static TypeFieldCollection Addresses
        {
            get { return Fields.CustomerAddress; }
        }
        
    }
}
