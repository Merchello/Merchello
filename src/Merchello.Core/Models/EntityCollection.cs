namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The entity collection.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class EntityCollection : DeployableEntity, IEntityCollection
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The entity type field key.
        /// </summary>
        private Guid _entityTfKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The dynamic collection.
        /// </summary>
        private Guid _prodiverKey;

        /// <summary>
        /// The _parent key.
        /// </summary>
        private Guid? _parentKey;

        /// <summary>
        /// The sort order.
        /// </summary>
        private int _sortOrder;

        /// <summary>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        /// <summary>
        /// Gets a value indicating whether the collection is represented as a filter.
        /// </summary>
        private bool _isFilter;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection"/> class.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="providerKey">
        /// The provider Key.
        /// </param>
        public EntityCollection(Guid entityTfKey, Guid providerKey)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityKey");
            Ensure.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _entityTfKey = entityTfKey;
            _prodiverKey = providerKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? ParentKey
        {
            get
            {
                return _parentKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _parentKey, _ps.Value.ParentKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid EntityTfKey
        {
            get
            {
                return _entityTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _entityTfKey, _ps.Value.EntityTfKeySelector);
            }
        }

        /// <inheritdoc/>
        public EntityType EntityType
        {
            get
            {
                return EnumTypeFieldConverter.EntityType.GetTypeField(EntityTfKey);
            }             
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _sortOrder, _ps.Value.SortOrderSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ProviderKey
        {
            get
            {
                return _prodiverKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _prodiverKey, _ps.Value.ProviderKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool IsFilter
        {
            get
            {
                return _isFilter;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _isFilter, _ps.Value.IsFilterSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Handles the extended data collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The entity type field key selector.
            /// </summary>
            public readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid>(x => x.EntityTfKey);

            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, string>(x => x.Name);

            /// <summary>
            /// The provider key selector.
            /// </summary>
            public readonly PropertyInfo ProviderKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid>(x => x.ProviderKey);

            /// <summary>
            /// The parent key selector.
            /// </summary>
            public readonly PropertyInfo ParentKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid?>(x => x.ParentKey);

            /// <summary>
            /// The sort info selector.
            /// </summary>
            public readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, int>(x => x.SortOrder);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, ExtendedDataCollection>(x => x.ExtendedData);

            /// <summary>
            /// The is filter selector.
            /// </summary>
            public readonly PropertyInfo IsFilterSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, bool>(x => x.IsFilter);
        }
    }
}