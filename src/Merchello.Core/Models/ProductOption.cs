using System;
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
        private static readonly PropertyInfo ChoicesSelector = ExpressionHelper.GetPropertyInfo<ProductOption, ProductAttributeCollection>(x => x.Choices);

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
        public ProductAttributeCollection Choices {
            get { return _choices; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _choices = value;
                    return _choices;
                }, _choices, ChoicesSelector);
            }
        }
    }
}