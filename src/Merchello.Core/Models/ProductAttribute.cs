using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product attribute
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ProductAttribute : IdEntity, IProductAttribute
    {
        private int _optionId;
        private string _name;
        private string _sku;
        private int _sortOrder;

        public ProductAttribute(string name, string sku)
            : this(0, name, sku)
        {}

        public ProductAttribute(int optionId, string name, string sku)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");

            _optionId = optionId;
            _name = name;
            _sku = sku;
        }

        private static readonly PropertyInfo OptionIdSelector = ExpressionHelper.GetPropertyInfo<ProductAttribute, int>(x => x.OptionId);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductAttribute, string>(x => x.Name);
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<ProductAttribute, string>(x => x.Sku);
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<ProductAttribute, int>(x => x.SortOrder);

        /// <summary>
        /// The id of the option which defines the attribute group this attribute belongs to
        /// </summary>
        [DataMember]
        public int OptionId {
            get { return _optionId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _optionId = value;
                    return _optionId;
                }, _optionId, NameSelector);
            }
        }

        /// <summary>
        /// The name of the attribute
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
        /// The suggested sku concatenation
        /// </summary>
        [DataMember]
        public string Sku {
            get { return _sku; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                    {
                        _sku = value;
                        return _sku;
                    }, _sku, SkuSelector);
            } 
        }

        /// <summary>
        /// The order in which to list the product attribute with respect to the product option
        /// </summary>
        [DataMember]
        public int SortOrder {
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
    }
}