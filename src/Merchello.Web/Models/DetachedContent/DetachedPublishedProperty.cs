namespace Merchello.Web.Models.DetachedContent
{
    using System;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// Represents a <see cref="IPublishedProperty"/>.
    /// </summary>
    internal class DetachedPublishedProperty : IDetachedPublishedProperty
    {
        /// <summary>
        /// The _culture name.
        /// </summary>
        private readonly string _cultureName;

        /// <summary>
        /// The property type.
        /// </summary>
        private readonly PublishedPropertyType _propertyType;

        /// <summary>
        /// The raw value.
        /// </summary>
        private readonly object _rawValue;

        /// <summary>
        /// The source value.
        /// </summary>
        private readonly Lazy<object> _sourceValue;

        /// <summary>
        /// The object value.
        /// </summary>
        private readonly Lazy<object> _objectValue;

        /// <summary>
        /// The xpath value.
        /// </summary>
        private readonly Lazy<object> _xpathValue;

        /// <summary>
        /// A value indicating whether this is a preview.
        /// </summary>
        private readonly bool _isPreview;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedPublishedProperty"/> class.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="cultureName">
        /// The culture name
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public DetachedPublishedProperty(PublishedPropertyType propertyType, string cultureName, object value)
            : this(propertyType, cultureName, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedPublishedProperty"/> class.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="cultureName">
        /// The culture name
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="isPreview">
        /// The is preview.
        /// </param>
        public DetachedPublishedProperty(PublishedPropertyType propertyType, string cultureName, object value, bool isPreview)
        {
            Mandate.ParameterNotNullOrEmpty(cultureName, "cultureName");

            this._propertyType = propertyType;
            this._isPreview = isPreview;
            this._rawValue = value;
            this._cultureName = cultureName;
            this._sourceValue = new Lazy<object>(() => this._propertyType.ConvertDataToSource(this._rawValue, this._isPreview));
            this._objectValue = new Lazy<object>(() => this._propertyType.ConvertSourceToObject(this._sourceValue.Value, this._isPreview));
            this._xpathValue = new Lazy<object>(() => this._propertyType.ConvertSourceToXPath(this._sourceValue.Value, this._isPreview));
        }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        public string CultureName
        {
            get
            {
                return this._cultureName;
            }
        }

        /// <summary>
        /// Gets the property type alias.
        /// </summary>
        public string PropertyTypeAlias
        {
            get
            {
                return this._propertyType.PropertyTypeAlias;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has value.
        /// </summary>
        public bool HasValue
        {
            get { return this.DataValue != null && this.DataValue.ToString().Trim().Length > 0; }
        }

        /// <summary>
        /// Gets the data value.
        /// </summary>
        public object DataValue
        {
            get { return this._rawValue; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value
        {
            get { return this._objectValue.Value; }
        }

        /// <summary>
        /// Gets the x path value.
        /// </summary>
        public object XPathValue
        {
            get { return this._xpathValue.Value; }
        }
    }
}