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
        /// The name of the component.
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
        /// The name of the component.
        /// </param>
        /// <param name="description">
        /// A description of the component.
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
        /// The name of the component.
        /// </param>
        /// <param name="description">
        /// A description of the component.
        /// </param>
        /// <param name="editorView">
        /// The editor view for the component
        /// </param>
        public OfferComponentAttribute(string key, string name, string description, string editorView)
            : this(key, name, description, editorView, null)
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
        /// A description of the component.
        /// </param>
        /// <param name="editorView">
        /// The editor view.  If set the back office will assume that a dialog should be added to associate additional configuration values with the component.
        /// </param>
        /// <param name="restrictToType">
        /// Restricts usage of the component to a particular type of <see cref="OfferComponentBase"/>.
        /// </param>
        public OfferComponentAttribute(string key, string name, string description, string editorView, Type restrictToType)
        {
            Ensure.ParameterNotNullOrEmpty(key, "key");
            Ensure.ParameterNotNullOrEmpty(name, "name");

            this.Key = new Guid(key);
            this.Name = name;
            this.Description = description;
            this.EditorView = editorView;
            this.RestrictToType = restrictToType;
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

        /// <summary>
        /// Gets or sets the restrict to type.
        /// </summary>
        public Type RestrictToType { get; set; }
    }
}