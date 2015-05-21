namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// Used to decorate Discount Rule classes that require a back office Angular dialog for additional configuration.
    /// </summary>
    public class OfferComponentAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The required key (GUID).  This should be a new GUID to represent this class.
        /// </param>
        /// <param name="name">
        /// The name of the constraint.
        /// </param>
        public OfferComponentAttribute(string key, string name)
            : this(key, name, string.Empty)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The required key (GUID).  This should be a new GUID to represent this class.
        /// </param>
        /// <param name="name">
        /// The name of the constraint.
        /// </param>
        /// <param name="description">
        /// A description of the constraint.
        /// </param>
        public OfferComponentAttribute(string key, string name, string description)
            : this(key, name, description, string.Empty)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentAttribute"/> class.
        /// </summary>
        /// <param name="key">
        /// The required key (GUID).  This should be a new GUID to represent this class.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// A description of the constraint.
        /// </param>
        /// <param name="editorView">
        /// The editor view.  If set the back office will assume that a dialog should be added to associate additional configuration values with the constraint.
        /// </param>
        public OfferComponentAttribute(string key, string name, string description, string editorView)
        {
            Mandate.ParameterNotNullOrEmpty(key, "key");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            this.Key = new Guid(key);
            this.Name = name;
            this.Description = description;
            this.EditorView = editorView;
        }

        #endregion

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the editor view.
        /// </summary>
        public string EditorView { get; set; }
    }
}