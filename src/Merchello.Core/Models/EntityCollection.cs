namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;

    /// <summary>
    /// The entity collection.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class EntityCollection : DeployableEntity, IEntityCollection
    {
        /// <summary>
        /// The entity type field key selector.
        /// </summary>
        private static readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid>(x => x.EntityTfKey);

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, string>(x => x.Name);

        /// <summary>
        /// The provider key selector.
        /// </summary>
        private static readonly PropertyInfo ProviderKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid>(x => x.ProviderKey);

        /// <summary>
        /// The parent key selector.
        /// </summary>
        private static readonly PropertyInfo ParentKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid?>(x => x.ParentKey);

        /// <summary>
        /// The sort info selector.
        /// </summary>
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, int>(x => x.SortOrder);

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, ExtendedDataCollection>(x => x.ExtendedData);

        /// <summary>
        /// The is filter selector.
        /// </summary>
        private static readonly PropertyInfo IsFilterSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, bool>(x => x.IsFilter);

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
            Mandate.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _entityTfKey = entityTfKey;
            _prodiverKey = providerKey;
        }

        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [DataMember]
        public Guid? ParentKey
        {
            get
            {
                return _parentKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                   o =>
                   {
                       _parentKey = value;
                       return _parentKey;
                   },
                   _parentKey,
                   ParentKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        public Guid EntityTfKey
        {
            get
            {
                return _entityTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _entityTfKey = value;
                        return _entityTfKey;
                    },
                    _entityTfKey,
                    EntityTfKeySelector);
            }
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public EntityType EntityType
        {
            get
            {
                return EnumTypeFieldConverter.EntityType.GetTypeField(EntityTfKey);
            }             
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                   o =>
                   {
                       _name = value;
                       return _name;
                   },
                   _name,
                   NameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [DataMember]
        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                  o =>
                  {
                      _sortOrder = value;
                      return _sortOrder;
                  },
                  _sortOrder,
                  SortOrderSelector);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic collection.
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get
            {
                return _prodiverKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                   o =>
                   {
                       _prodiverKey = value;
                       return _prodiverKey;
                   },
                   _prodiverKey,
                   ProviderKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is filter.
        /// </summary>
        [DataMember]
        public bool IsFilter
        {
            get
            {
                return _isFilter;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                   o =>
                   {
                       _isFilter = value;
                       return _isFilter;
                   },
                   _isFilter,
                   IsFilterSelector);
            }
        }

        /// <summary>
        /// Gets or sets the extended data collection.
        /// </summary>
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
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }
}