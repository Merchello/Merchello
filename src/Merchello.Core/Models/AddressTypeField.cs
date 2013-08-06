using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Identifies an address as either residential or commercial for shipping estimations 
    /// </summary>
    public class AddressTypeField : TypeFieldProxyBase
    {
        /// <summary>
        /// Indicates the address is a residential address
        /// </summary>
        public static ITypeField Residential
        {
            get { return GetTypeField(Addresses["Residential"]); }
        }

        /// <summary>
        /// Indicates the address is a commercial address
        /// </summary>
        public static ITypeField Commercial
        {
            get { return GetTypeField(Addresses["Commercial"]); }
        }

        /// <summary>
        /// Returns a custom address or NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom address</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        public new static ITypeField Custom(string alias)
        {
            return GetTypeField(Addresses[alias]);
        }

        private static TypeFieldCollection Addresses
        {
            get { return Fields.CustomerAddress; }
        }

    }
}
