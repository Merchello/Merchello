namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a product option.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public sealed class ProductOption : Entity, IProductOption
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// A value indicating whether or not it is required.
        /// </summary>
        private bool _required;

        /// <summary>
        /// The value indicating whether or not the option is a shared option.
        /// </summary>
        private bool _shared;

        /// <summary>
        /// The sort order.
        /// </summary>
        private int _sortOrder;

        /// <summary>
        /// The UI option.
        /// </summary>
        private string _uiOption;

        /// <summary>
        /// The use name.
        /// </summary>
        private string _useName;

        /// <summary>
        /// The detached content type key.
        /// </summary>
        private Guid? _detachedContentTypeKey;

        /// <summary>
        /// The option choices collection.
        /// </summary>
        private ProductAttributeCollection _choices;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOption"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public ProductOption(string name)
            : this(name, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOption"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="required">
        /// The required.
        /// </param>
        internal ProductOption(string name, bool required)
            : this(name, required, new ProductAttributeCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOption"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="required">
        /// The required.
        /// </param>
        /// <param name="choices">
        /// The choices.
        /// </param>
        internal ProductOption(string name, bool required, ProductAttributeCollection choices)
        {
            // TODO RSS review this
            // This is required so that we can create attributes from the WebApi without a lot of             
            // round trip traffic to the db to generate the Key(s).  Key is virtual so also forces
            // this class to be sealed
            Key = Guid.NewGuid();
            HasIdentity = false;

            _name = name;
            _required = required;
            _choices = choices;
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
        public string UseName
        {
            get
            {
                return _useName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _useName, _ps.Value.UseNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Required
        {
            get
            {
                return _required;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _required, _ps.Value.RequiredSelector);
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

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _sortOrder, _ps.Value.SortOrderSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Shared
        {
            get
            {
                return _shared;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shared, _ps.Value.SharedSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? DetachedContentTypeKey
        {
            get
            {
                return _detachedContentTypeKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _detachedContentTypeKey, _ps.Value.DetachedContentTypeKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string UiOption
        {
            get
            {
                return _uiOption;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _uiOption, _ps.Value.UiOptionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public ProductAttributeCollection Choices
        {
            get
            {

                return _choices;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _choices = value;
                _choices.CollectionChanged += ChoiceCollectionChanged;       
            }
        }

        /// <inheritdoc/>
        public IProductOption Clone()
        {
            var choices = this.Choices.Select(x => x.Clone()).OrderBy(x => x.SortOrder);

            var o = (ProductOption)this.MemberwiseClone();

            var atts = new ProductAttributeCollection();
            foreach (var c in choices)
            {
                atts.Add(c);
            }

            o.Choices = atts;
            o.ResetDirtyProperties();

            return o;
        }

        /// <summary>
        /// Handles the Choice Collection Changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChoiceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(_ps.Value.ProductAttributesChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductOption, string>(x => x.Name);

            /// <summary>
            /// The required selector.
            /// </summary>
            public readonly PropertyInfo RequiredSelector = ExpressionHelper.GetPropertyInfo<ProductOption, bool>(x => x.Required);

            /// <summary>
            /// The sort order selector.
            /// </summary>
            public readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<ProductOption, int>(x => x.SortOrder);

            /// <summary>
            /// The shared selector.
            /// </summary>
            public readonly PropertyInfo SharedSelector = ExpressionHelper.GetPropertyInfo<ProductOption, bool>(x => x.Shared);

            /// <summary>
            /// The use name selector.
            /// </summary>
            public readonly PropertyInfo UseNameSelector = ExpressionHelper.GetPropertyInfo<ProductOption, string>(x => x.UseName);

            /// <summary>
            /// The UI option selector.
            /// </summary>
            public readonly PropertyInfo UiOptionSelector = ExpressionHelper.GetPropertyInfo<ProductOption, string>(x => x.UiOption);

            /// <summary>
            /// The detached content type key selector.
            /// </summary>
            public readonly PropertyInfo DetachedContentTypeKeySelector = ExpressionHelper.GetPropertyInfo<ProductOption, Guid?>(x => x.DetachedContentTypeKey);

            /// <summary>
            /// The product attribute collection changed selector.
            /// </summary>
            public readonly PropertyInfo ProductAttributesChangedSelector = ExpressionHelper.GetPropertyInfo<ProductOption, ProductAttributeCollection>(x => x.Choices);
        }
    }
}