using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductOption : IdEntity, IProductOption
    {
        private string _name;
        private bool _required;
        private int _sortOrder;
        private ProductAttributeCollection _choices;

        public ProductOption(string name)
            : this(name, true)
        { }

        internal ProductOption(string name, bool required)
            : this(name, required, new ProductAttributeCollection())
        { }

        internal ProductOption(string name, bool required, ProductAttributeCollection choices)
        {
            _name = name;
            _required = required;
            _choices = choices;
        }

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductOption, string>(x => x.Name);
        private static readonly PropertyInfo RequiredSelector = ExpressionHelper.GetPropertyInfo<ProductOption, bool>(x => x.Required);
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<ProductOption, int>(x => x.SortOrder);
        private static readonly PropertyInfo ChoicesChangedSelector = ExpressionHelper.GetPropertyInfo<ProductOption, ProductAttributeCollection>(x => x.Choices);

        protected void ProductAttributeChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ChoicesChangedSelector);
        }

        /// <summary>
        /// The name of the option
        /// </summary>
        [DataMember]
        public string Name {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not it is required to select an option in order to purchase the associated product.
        /// </summary>
        /// <remarks>
        /// If true - a product item to product attribute relation is created defines the composition of a product item
        /// </remarks>
        [DataMember]
        public bool Required
        {
            get { return _required; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _required = value;
                    return _required;
                }, _required, RequiredSelector);
            }
        }

        /// <summary>
        /// The order in which to list product option with respect to its product association
        /// </summary>
        [DataMember]
        public int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _sortOrder = value;
                    return _sortOrder;
                }, _sortOrder, SortOrderSelector);
            }
        }


        /// <summary>
        /// The choices (product attributes) associated with this option
        /// </summary>
        [DataMember]
        public ProductAttributeCollection Choices {
            get { return _choices; }
            set { 
                _choices = value;
                _choices.CollectionChanged += ProductAttributeChanged;
            }
        }
    }
}