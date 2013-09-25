using System;
using System.Linq;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Identifies an address as either residential or commercial for shipping estimations 
    /// </summary>
    internal sealed class AddressTypeField : TypeFieldMapper<AddressType>, IAddressTypeField
    {
        internal AddressTypeField()
        {
            if(CachedTypeFields.IsEmpty) BuildCache();
        }

#region Overrides TypeFieldMapper<AddressType>


        internal override void BuildCache()
        {
            AddUpdateCache(AddressType.Residential, new TypeField("Residential", "Residential", new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46")));
            AddUpdateCache(AddressType.Commercial,  new TypeField("Commercial", "Commercial", new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360")));
        }

#endregion

        /// <summary>
        /// Indicates the address is a residential address
        /// </summary>
        public ITypeField Residential
        {
            get { return GetTypeField(AddressType.Residential); }
        }

        /// <summary>
        /// Indicates the address is a commercial address
        /// </summary>
        public ITypeField Commercial
        {
            get { return GetTypeField(AddressType.Commercial); }
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
