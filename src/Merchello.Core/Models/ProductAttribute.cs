namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;
    using EntityBase;

    using Merchello.Core.Models.DetachedContent;

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
            Ensure.ParameterNotNullOrEmpty(name, "name");
            Ensure.ParameterNotNullOrEmpty(sku, "sku");
            
            // This is required so that we can create attributes from the WebApi without a lot of 
            // round trip traffic to the db to generate the Key(s).  Key is virtual so also forces
            // this class to be sealed
            Key = Guid.NewGuid();
            HasIdentity = false;
            DetachedDataValues = new DetachedDataValuesCollection();
            Name = name;
            Sku = sku;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid OptionKey { get; set; }


        /// <inheritdoc/>
        [DataMember]
        public string Name { get; set; }


        /// <inheritdoc/>
        [DataMember]
        public string Sku { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public int SortOrder { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public bool IsDefaultChoice { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public DetachedDataValuesCollection DetachedDataValues { get; internal set; }

        /// <inheritdoc/>
        public IProductAttribute Clone()
        {
            return (ProductAttribute)this.MemberwiseClone();
        }
    }
}