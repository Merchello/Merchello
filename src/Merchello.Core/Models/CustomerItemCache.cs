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
    internal class CustomerItemCache : IdEntity, ICustomerItemCache
    {
        private Guid _consumerKey;
        private Guid _itemCacheTfKey;
        private readonly LineItemCollection _items;
 
        public CustomerItemCache(Guid consumerKey, CustomerItemCacheType customerItemCacheType)
            : this(consumerKey, EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(customerItemCacheType).TypeKey, new LineItemCollection())
        { }

        internal CustomerItemCache(Guid consumerKey, Guid itemCacheTfKey)
            : this(consumerKey, itemCacheTfKey, new LineItemCollection())
        { }

        public CustomerItemCache(Guid consumerKey, CustomerItemCacheType customerItemCacheType, LineItemCollection items)
            : this(consumerKey, EnumTypeFieldConverter.CustomerItemItemCache.GetTypeField(customerItemCacheType).TypeKey, items)
        { }

        internal CustomerItemCache(Guid consumerKey, Guid itemCacheTfKey, LineItemCollection items)
        {
            Mandate.ParameterCondition(consumerKey != Guid.Empty, "consumerKey");
            Mandate.ParameterCondition(itemCacheTfKey != Guid.Empty, "customerRegisterTfKey");
            Mandate.ParameterNotNull(items, "items");

            _consumerKey = consumerKey;
            _itemCacheTfKey = itemCacheTfKey;
            _items = items;
        }
        
        private static readonly PropertyInfo CustomerRegistrySelector = ExpressionHelper.GetPropertyInfo<CustomerItemCache, Guid>(x => x.ConsumerKey);
        private static readonly PropertyInfo RegistryTfKeySelector = ExpressionHelper.GetPropertyInfo<CustomerItemCache, Guid>(x => x.ItemCacheTfKey);
        
        /// <summary>
        /// The <see cref="IConsumer"/> key
        /// </summary>
        [DataMember]
        public Guid ConsumerKey
        {
            get { return _consumerKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _consumerKey = value;
                    return _consumerKey;
                }, _consumerKey, CustomerRegistrySelector);
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
                }, _itemCacheTfKey, RegistryTfKeySelector);
            }
        }

        /// <summary>
        /// The <see cref="CustomerItemCacheType"/> of the customer registry
        /// </summary>
        [DataMember]
        public CustomerItemCacheType CustomerItemCacheType
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
            get { return _items; }
        }
    }
}