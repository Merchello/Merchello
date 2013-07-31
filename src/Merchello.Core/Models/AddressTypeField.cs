using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Identifies an address as either residential or commercial for shipping estimations 
    /// </summary>
    public class AddressTypeField : TypeFieldBase
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


        private static TypeFieldCollection Addresses
        {
            get { return Fields.CustomerAddress; }
        }

    }
}
