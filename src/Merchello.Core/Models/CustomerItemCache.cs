using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer item register
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerItemCache : IdEntity, ICustomerItemCache
    {
        private Guid _customerKey;
        private Guid _itemCacheTfKey;
        private LineItemCollection _items;
 
        public CustomerItemCache(Guid customerKey, ItemCacheType itemCacheType)
            : this(customerKey, EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(itemCacheType).TypeKey, new LineItemCollection())
        { }

        internal CustomerItemCache(Guid customerKey, Guid itemCacheTfKey)
            : this(customerKey, itemCacheTfKey, new LineItemCollection())
        { }

        public CustomerItemCache(Guid customerKey, ItemCacheType itemCacheType, LineItemCollection items)
            : this(customerKey, EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(itemCacheType).TypeKey, items)
        { }

        internal CustomerItemCache(Guid customerKey, Guid itemCacheTfKey, LineItemCollection items)
        {
            Mandate.ParameterCondition(customerKey != Guid.Empty, "consumerKey");
            Mandate.ParameterCondition(itemCacheTfKey != Guid.Empty, "itemCacheTfKey");
            Mandate.ParameterNotNull(items, "items");

            _customerKey = customerKey;
            _itemCacheTfKey = itemCacheTfKey;
            _items = items;
        }
        
        private static readonly PropertyInfo CustomerRegistrySelector = ExpressionHelper.GetPropertyInfo<CustomerItemCache, Guid>(x => x.CustomerKey);
        private static readonly PropertyInfo ItemCacheTfKeySelector = ExpressionHelper.GetPropertyInfo<CustomerItemCache, Guid>(x => x.ItemCacheTfKey);
        
        /// <summary>
        /// The <see cref="ICustomer"/> key
        /// </summary>
        [DataMember]
        public Guid CustomerKey
        {
            get { return _customerKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _customerKey = value;
                    return _customerKey;
                }, _customerKey, CustomerRegistrySelector);
            }
        }

        /// <summary>
        /// The registry type field <see cref="ITypeField"/> guid typeKey
        /// </summary>
        [DataMember]
        public Guid ItemCacheTfKey
        {
            get { return _itemCacheTfKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _itemCacheTfKey = value;
                    return _itemCacheTfKey;
                }, _itemCacheTfKey, ItemCacheTfKeySelector);
            }
        }

        /// <summary>
        /// The <see cref="ItemCacheType"/> of the customer registry
        /// </summary>
        [DataMember]
        public ItemCacheType ItemCacheType
        {
            get { return EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(_itemCacheTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    _itemCacheTfKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// The <see cref="IOrderLineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        public LineItemCollection Items {
            get
            {
                return _items;                 
            }
            internal set
            {
                _items = value;
            }
        }
    }
}