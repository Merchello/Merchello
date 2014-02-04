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
    public class ItemCache : Entity, IItemCache
    {
        private Guid _entityKey;
        private Guid _itemCacheTfKey;
        private LineItemCollection _items;
 
        public ItemCache(Guid entityKey, ItemCacheType itemCacheType)
            : this(entityKey, EnumTypeFieldConverter.ItemItemCache.GetTypeField(itemCacheType).TypeKey, new LineItemCollection())
        { }

        internal ItemCache(Guid entityKey, Guid itemCacheTfKey)
            : this(entityKey, itemCacheTfKey, new LineItemCollection())
        { }

        public ItemCache(Guid entityKey, ItemCacheType itemCacheType, LineItemCollection items)
            : this(entityKey, EnumTypeFieldConverter.ItemItemCache.GetTypeField(itemCacheType).TypeKey, items)
        { }

        internal ItemCache(Guid entityKey, Guid itemCacheTfKey, LineItemCollection items)
        {
            Mandate.ParameterCondition(entityKey != Guid.Empty, "entityKey");
            Mandate.ParameterCondition(itemCacheTfKey != Guid.Empty, "itemCacheTfKey");
            Mandate.ParameterNotNull(items, "items");

            _entityKey = entityKey;
            _itemCacheTfKey = itemCacheTfKey;
            _items = items;
        }
        
        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<ItemCache, Guid>(x => x.EntityKey);
        private static readonly PropertyInfo ItemCacheTfKeySelector = ExpressionHelper.GetPropertyInfo<ItemCache, Guid>(x => x.ItemCacheTfKey);
        
        /// <summary>
        /// The key of the entity associated with the item cache
        /// </summary>
        [DataMember]
        public Guid EntityKey
        {
            get { return _entityKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _entityKey = value;
                    return _entityKey;
                }, _entityKey, EntityKeySelector);
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
        /// The <see cref="ItemCacheType"/> of the item cache
        /// </summary>
        [DataMember]
        public ItemCacheType ItemCacheType
        {
            get { return EnumTypeFieldConverter.ItemItemCache.GetTypeField(_itemCacheTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.ItemItemCache.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    _itemCacheTfKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// The <see cref="ILineItem"/>s in the customer registry
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