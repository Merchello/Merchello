namespace Merchello.Web.Models.DetachedContent
{
    using System;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    internal class DetachedPublishedProperty : IPublishedProperty
    {
        private readonly PublishedPropertyType _propertyType;
        private readonly object _rawValue;
        private readonly Lazy<object> _sourceValue;
        private readonly Lazy<object> _objectValue;
        private readonly Lazy<object> _xpathValue;
        private readonly bool _isPreview;

        public DetachedPublishedProperty(PublishedPropertyType propertyType, object value)
            : this(propertyType, value, false)
        {
        }

        public DetachedPublishedProperty(PublishedPropertyType propertyType, object value, bool isPreview)
        {
            this._propertyType = propertyType;
            this._isPreview = isPreview;

            this._rawValue = value;

            this._sourceValue = new Lazy<object>(() => this._propertyType.ConvertDataToSource(this._rawValue, this._isPreview));
            this._objectValue = new Lazy<object>(() => this._propertyType.ConvertSourceToObject(this._sourceValue.Value, this._isPreview));
            this._xpathValue = new Lazy<object>(() => this._propertyType.ConvertSourceToXPath(this._sourceValue.Value, this._isPreview));
        }

        public string PropertyTypeAlias
        {
            get
            {
                return this._propertyType.PropertyTypeAlias;
            }
        }

        public bool HasValue
        {
            get { return this.DataValue != null && this.DataValue.ToString().Trim().Length > 0; }
        }

        public object DataValue { get { return this._rawValue; } }

        public object Value { get { return this._objectValue.Value; } }

        public object XPathValue { get { return this._xpathValue.Value; } }
    }
}