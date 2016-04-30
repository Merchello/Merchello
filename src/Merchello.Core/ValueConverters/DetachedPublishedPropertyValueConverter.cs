namespace Merchello.Core.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.DetachedContent;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Editors;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;

    /// <summary>
    /// A converter to assist in saving detached property data correctly.
    /// </summary>
    internal class DetachedPublishedPropertyValueConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        private static DetachedPublishedPropertyValueConverter _instance;

        /// <summary>
        /// The <see cref="IDataTypeService"/>.
        /// </summary>
        private readonly IDataTypeService _dataTypeService;

        /// <summary>
        /// The <see cref="ContentTypeService"/>.
        /// </summary>
        private readonly IContentTypeService _contentTypeService;

        /// <summary>
        /// A value to indicate if the converter singleton is ready.
        /// </summary>
        /// <remarks>
        /// Can be removed eventually when Integration tests get refactored with an instantiated ApplicationContext but to
        /// </remarks>
        private readonly bool _ready;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedPublishedPropertyValueConverter"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The <see cref="ApplicationContext"/>.
        /// </param>
        internal DetachedPublishedPropertyValueConverter(ApplicationContext applicationContext)
        {
            if (applicationContext != null)
            {
                _contentTypeService = applicationContext.Services.ContentTypeService;
                _dataTypeService = applicationContext.Services.DataTypeService;
                _ready = true;
            }
            else
            {
                _ready = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has current.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return _instance != null;
            }
        }

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        public static DetachedPublishedPropertyValueConverter Current
        {
            get
            {
                return _instance;
            }

            internal set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// A method to deserialize the string value that has been saved in the content editor
        ///             to an object to be stored in the database.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="detachedContentValue">
        /// The detached content value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// By default this will attempt to automatically convert the string value to the value type supplied by ValueType.
        /// 
        ///             If overridden then the object returned must match the type supplied in the ValueType, otherwise persisting the
        ///             value to the DB will fail when it tries to validate the value type.
        /// 
        /// </remarks>
        public string ConvertEditorToDb(IContentType contentType, KeyValuePair<string, object> detachedContentValue)
        {
            if (contentType == null || !_ready) return detachedContentValue.Value.ToString();

            var propType = contentType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias == detachedContentValue.Key);
            if (propType == null)
            {
                return string.Empty;
            }

            // Fetch the property types prevalue
            var propPreValues = _dataTypeService.GetPreValuesCollectionByDataTypeId(propType.DataTypeDefinitionId);

            // Lookup the property editor
            var propEditor = PropertyEditorResolver.Current.GetByAlias(propType.PropertyEditorAlias);

            // Create a fake content property data object
            var contentPropData = new ContentPropertyData(detachedContentValue.Value, propPreValues, new Dictionary<string, object>());

            // Get the property editor to do it's conversion
            var newValue = propEditor.ValueEditor.ConvertEditorToDb(contentPropData, detachedContentValue.Value);

            // Store the value back
            var value = newValue == null ? null : JToken.FromObject(newValue);

            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// A method used to format the database value to a value that can be used by the editor
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="detachedContentValue">
        /// The detached content value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <remarks>
        /// The object returned will automatically be serialized into JSON notation. For most property editors
        ///             the value returned is probably just a string but in some cases a JSON structure will be returned.
        /// 
        /// </remarks>
        public KeyValuePair<string,string> ConvertDbToEditor(IContentType contentType, KeyValuePair<string, string> detachedContentValue)
        {
            if (contentType == null || !_ready) return detachedContentValue;

            var propType = contentType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias == detachedContentValue.Key);
            if (propType == null)
            {
                return detachedContentValue;
            }


            var rawValue = !JsonHelper.IsJsonObject(detachedContentValue.Value)
                               ? JsonConvert.DeserializeObject(detachedContentValue.Value)
                               : detachedContentValue.Value;

            //// Adapted from Nested Content
            // Create a fake property using the property to add the stored value
            var prop = new Property(propType, rawValue);

            // Lookup the property editor
            var propEditor = PropertyEditorResolver.Current.GetByAlias(propType.PropertyEditorAlias);

            // Get the editor to do it's conversion
            var editorObject = propEditor.ValueEditor.ConvertDbToEditor(prop, propType, _dataTypeService);
            var json = JsonConvert.SerializeObject(editorObject);
            return new KeyValuePair<string, string>(detachedContentValue.Key, json);
        }

        /// <summary>
        /// Gets <see cref="IContentType"/> from <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <returns>
        /// The <see cref="IContentType"/>.
        /// </returns>
        internal IContentType GetContentTypeFromDetachedContentType(IDetachedContentType detachedContentType)
        {
            if (detachedContentType == null || detachedContentType.ContentTypeKey == null) return null;

            return _contentTypeService.GetContentType(detachedContentType.ContentTypeKey.Value);
        }
    }
}