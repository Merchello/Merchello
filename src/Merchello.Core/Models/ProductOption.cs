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
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductOption, string>(x => x.Name);

        /// <summary>
        /// The required selector.
        /// </summary>
        private static readonly PropertyInfo RequiredSelector = ExpressionHelper.GetPropertyInfo<ProductOption, bool>(x => x.Required);

        /// <summary>
        /// The shared selector.
        /// </summary>
        private static readonly PropertyInfo SharedSelector = ExpressionHelper.GetPropertyInfo<ProductOption, bool>(x => x.Shared);

        /// <summary>
        /// The UI option selector.
        /// </summary>
        private static readonly PropertyInfo UiOptionSelector = ExpressionHelper.GetPropertyInfo<ProductOption, string>(x => x.UiOption);

        /// <summary>
        /// The detached content type key selector.
        /// </summary>
        private static readonly PropertyInfo DetachedContentTypeKeySelector = ExpressionHelper.GetPropertyInfo<ProductOption, Guid?>(x => x.DetachedContentTypeKey);

        /// <summary>
        /// The product attribute collection changed selector.
        /// </summary>
        private static readonly PropertyInfo ProductAttributesChangedSelector = ExpressionHelper.GetPropertyInfo<ProductOption, ProductAttributeCollection>(x => x.Choices);

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
        /// The UI option.
        /// </summary>
        private string _uiOption;

        /// <summary>
        /// The detached content type key.
        /// </summary>
        private Guid? _detachedContentTypeKey;

        /// <summary>
        /// The option choices collection.
        /// </summary>
        private ProductAttributeCollection _choices;

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
        /// Gets or sets the use name.
        /// </summary>
        /// <remarks>
        /// This is the name associated with the product option when the option is shared.
        /// Provides an alternate, per use naming of the option.
        /// </remarks>
        public string UseName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not it is required to select an option in order to purchase the associated product.
        /// </summary>
        /// <remarks>
        /// If true - a product item to product attribute relation is created defines the composition of a product item
        /// </remarks>
        [DataMember]
        public bool Required
        {
            get
            {
                return _required;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _required = value;
                        return _required;
                    },
                _required,
                RequiredSelector);
            }
        }

        /// <summary>
        /// Gets or sets the order in which to list product option with respect to its product association
        /// </summary>
        /// <remarks>
        /// This field is stored in the product 2 product option association and is not valid for shared option list (it is populated when associated with a product) - cache value should always be 0.
        /// </remarks>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the option is shared.
        /// </summary>
        [DataMember]
        public bool Shared
        {
            get
            {
                return _shared;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _shared = value;
                        return _shared;
                    },
                _shared,
                SharedSelector);
            }
        }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        [DataMember]
        public Guid? DetachedContentTypeKey
        {
            get
            {
                return _detachedContentTypeKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _detachedContentTypeKey = value;
                        return _detachedContentTypeKey;
                    },
                _detachedContentTypeKey,
                DetachedContentTypeKeySelector);
            }
        }
        
        /// <summary>
        /// Gets or sets the UI option.
        /// </summary>
        [DataMember]
        public string UiOption
        {
            get
            {
                return _uiOption;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _uiOption = value;
                        return _uiOption;
                    },
                _uiOption,
                UiOptionSelector);
            }
        }

        /// <summary>
        /// Gets or sets the choices (product attributes) associated with this option
        /// </summary>
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

        /// <summary>
        /// Creates a clone of this option.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
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
            this.OnPropertyChanged(ProductAttributesChangedSelector);
        }
    }
}