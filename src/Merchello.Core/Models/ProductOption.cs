namespace Merchello.Core.Models
{
    using System;
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
                _name = value;
            }
        }

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
                _required = value;
            }
        }

        /// <summary>
        /// Gets or sets the order in which to list product option with respect to its product association
        /// </summary>
        /// TODO RSS this should be on the product association
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
                _shared = value;
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
                _choices = value;                
            }
        }
    }
}