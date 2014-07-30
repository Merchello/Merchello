namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;
    using EntityBase;

    /// <summary>
    /// Defines a product attribute
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public sealed class ProductAttribute : Entity, IProductAttribute
    {
        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The sku.
        /// </summary>
        private string _sku;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
        /// </param>
        public ProductAttribute(string name, string sku)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
            
            // This is required so that we can create attributes from the WebApi without a lot of 
            // round trip traffic to the db to generate the Key(s).  Key is virtual so also forces
            // this class to be sealed
            Key = Guid.NewGuid();
            HasIdentity = false;

            _name = name;
            _sku = sku;
        }

        /// <summary>
        /// Gets or sets id of the option which defines the attribute group this attribute belongs to
        /// </summary>
        [DataMember]
        public Guid OptionKey { get; set; }


        /// <summary>
        /// The name of the attribute
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
        /// The suggested sku concatenation
        /// </summary>
        [DataMember]
        public string Sku {
            get { return _sku; }
            set
            {
                _sku = value;
            } 
        }

        /// <summary>
        /// The order in which to list the product attribute with respect to the product option
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }


    }
}