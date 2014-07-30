namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;
    using TypeFields;

    /// <summary>
    /// Defines a customer item register
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ItemCache : VersionTaggedEntity, IItemCache
    {
        #region Fields

        /// <summary>
        /// The entity key selector.
        /// </summary>
        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<ItemCache, Guid>(x => x.EntityKey);

        /// <summary>
        /// The item cache tf key selector.
        /// </summary>
        private static readonly PropertyInfo ItemCacheTfKeySelector = ExpressionHelper.GetPropertyInfo<ItemCache, Guid>(x => x.ItemCacheTfKey);

        /// <summary>
        /// The entity key.
        /// </summary>
        private Guid _entityKey;

        /// <summary>
        /// The item cache type field key.
        /// </summary>
        private Guid _itemCacheTfKey;

        /// <summary>
        /// The line items collection
        /// </summary>
        private LineItemCollection _items;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCache"/> class.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="itemCacheType">
        /// The item cache type.
        /// </param>
        public ItemCache(Guid entityKey, ItemCacheType itemCacheType)
            : this(entityKey, EnumTypeFieldConverter.ItemItemCache.GetTypeField(itemCacheType).TypeKey, new LineItemCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCache"/> class.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="itemCacheType">
        /// The item cache type.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        public ItemCache(Guid entityKey, ItemCacheType itemCacheType, LineItemCollection items)
            : this(entityKey, EnumTypeFieldConverter.ItemItemCache.GetTypeField(itemCacheType).TypeKey, items)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCache"/> class.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="itemCacheTfKey">
        /// The item cache tf key.
        /// </param>
        internal ItemCache(Guid entityKey, Guid itemCacheTfKey)
            : this(entityKey, itemCacheTfKey, new LineItemCollection())
        {            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCache"/> class.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="itemCacheTfKey">
        /// The item cache tf key.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        internal ItemCache(Guid entityKey, Guid itemCacheTfKey, LineItemCollection items)
        {
            Mandate.ParameterCondition(entityKey != Guid.Empty, "entityKey");
            Mandate.ParameterCondition(itemCacheTfKey != Guid.Empty, "itemCacheTfKey");
            Mandate.ParameterNotNull(items, "items");

            _entityKey = entityKey;
            _itemCacheTfKey = itemCacheTfKey;
            _items = items;
        }               
        
        /// <summary>
        /// Gets or sets key of the entity associated with the item cache
        /// </summary>
        [DataMember]
        public Guid EntityKey
        {
            get
            {
                return _entityKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _entityKey = value;
                    return _entityKey;
                }, 
                _entityKey, 
                EntityKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the item cache type field <see cref="ITypeField"/> guid typeKey
        /// </summary>
        [DataMember]
        public Guid ItemCacheTfKey
        {
            get
            {
                return _itemCacheTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                    _itemCacheTfKey = value;
                    return _itemCacheTfKey;
                }, 
                _itemCacheTfKey, 
                ItemCacheTfKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ItemCacheType"/> of the item cache
        /// </summary>
        [DataMember]
        public ItemCacheType ItemCacheType
        {
            get
            {
                return EnumTypeFieldConverter.ItemItemCache.GetTypeField(_itemCacheTfKey);
            }

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
        /// Gets the <see cref="ILineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        public LineItemCollection Items 
        {
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