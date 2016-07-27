namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;
    using EntityBase;

    using Merchello.Core.Models.DetachedContent;

    using Umbraco.Core;

    /// <summary>
    /// Defines a product attribute
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public sealed class ProductAttribute : Entity, IProductAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
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

            Name = name;
            Sku = sku;
        }

        /// <summary>
        /// Gets or sets id of the option which defines the attribute group this attribute belongs to
        /// </summary>
        [DataMember]
        public Guid OptionKey { get; set; }


        /// <summary>
        /// Gets or sets the name of the attribute
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        

        /// <summary>
        /// Gets or sets suggested SKU concatenation
        /// </summary>
        [DataMember]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets order in which to list the product attribute with respect to the product option
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default choice.
        /// </summary>
        [DataMember]
        public bool IsDefaultChoice { get; set; }

        /// <summary>
        /// Gets the detached data values.
        /// </summary>
        [DataMember]
        public DetachedDataValuesCollection DetachedDataValues { get; internal set; }

        /// <summary>
        /// Creates a cloned copy of this object.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductAttribute"/>.
        /// </returns>
        public IProductAttribute Clone()
        {
            return (ProductAttribute)this.MemberwiseClone();
        }
    }
}