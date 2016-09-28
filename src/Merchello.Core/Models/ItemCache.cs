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
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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
        /// The item cache type field key.
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
        /// The item cache type field key.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        internal ItemCache(Guid entityKey, Guid itemCacheTfKey, LineItemCollection items)
        {
            Ensure.ParameterCondition(entityKey != Guid.Empty, "entityKey");
            Ensure.ParameterCondition(itemCacheTfKey != Guid.Empty, "itemCacheTfKey");
            Ensure.ParameterNotNull(items, "items");

            _entityKey = entityKey;
            _itemCacheTfKey = itemCacheTfKey;
            _items = items;
        }               
        
        /// <inheritdoc/>
        [DataMember]
        public Guid EntityKey
        {
            get
            {
                return _entityKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _entityKey, _ps.Value.EntityKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ItemCacheTfKey
        {
            get
            {
                return _itemCacheTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _itemCacheTfKey, _ps.Value.ItemCacheTfKeySelector);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Accept(ILineItemVisitor visitor)
        {
            this.Items.Accept(visitor);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The entity key selector.
            /// </summary>
            public readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<ItemCache, Guid>(x => x.EntityKey);

            /// <summary>
            /// The item cache type field key selector.
            /// </summary>
            public readonly PropertyInfo ItemCacheTfKeySelector = ExpressionHelper.GetPropertyInfo<ItemCache, Guid>(x => x.ItemCacheTfKey);

        }
    }
}