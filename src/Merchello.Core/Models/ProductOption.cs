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
    public sealed class ProductOption : Entity, IProductOption
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
        /// The name of the option
        /// </summary>
        [DataMember]
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
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
                _required = value;
            }
        }

        /// <summary>
        /// The order in which to list product option with respect to its product association
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }


        /// <summary>
        /// The choices (product attributes) associated with this option
        /// </summary>
        [DataMember]
        public ProductAttributeCollection Choices {
            get { return _choices; }
            set { 
                _choices = value;                
            }
        }
    }
}