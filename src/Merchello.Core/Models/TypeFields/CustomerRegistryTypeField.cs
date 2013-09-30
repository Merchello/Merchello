using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Indicates whether a shopping cart basket is either a "basket" or a "wishlist" representation
    /// </summary>
    internal class CustomerRegistryTypeField : TypeFieldMapper<CustomerItemRegisterType>, ICustomerRegistryTypeField
    {
        internal CustomerRegistryTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

#region Overrides TypeFieldMapper<BasketType>

        internal override sealed void BuildCache()
        {
            AddUpdateCache(CustomerItemRegisterType.Basket, new TypeField("Basket", "Standard Basket", new Guid("C53E3100-2DFD-408A-872E-4380383FAD35")));
            AddUpdateCache(CustomerItemRegisterType.Wishlist, new TypeField("Wishlist", "Wishlist", new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5")));
        }

        /// <summary>
        /// Returns a custom basket or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom basket</param>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Baskets[alias]);
        }

#endregion

        /// <summary>
        /// Default ecommerce basket
        /// </summary>
        public ITypeField Basket
        {
            get { return GetTypeField(CustomerItemRegisterType.Basket); }
        }

        /// <summary>
        /// Wishlist
        /// </summary>
        public ITypeField Wishlist
        {
            get { return GetTypeField(CustomerItemRegisterType.Wishlist); }
        }


        private static TypeFieldCollection Baskets
        {
            get { return Fields.Basket; }
        }

    }
}
