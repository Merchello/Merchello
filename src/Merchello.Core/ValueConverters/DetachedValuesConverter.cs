namespace Merchello.Core.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
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
    internal class DetachedValuesConverter
    {
        /// <summary>
        /// The singleton instance of the converter.
        /// </summary>
        private static DetachedValuesConverter _instance;

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
        /// Initializes a new instance of the <see cref="DetachedValuesConverter"/> class.
        /// </summary>
        /// <param name="applicationContext">
        /// The <see cref="ApplicationContext"/>.
        /// </param>
        internal DetachedValuesConverter(ApplicationContext applicationContext)
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
        public static DetachedValuesConverter Current
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
        /// Converts the detached values collection to property values for various usages depending on type passed.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="detachedContentValues">
        /// The detached content values.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type.
        /// </param>
        /// <returns>
        /// The converted values.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> Convert(IContentType contentType, IEnumerable<KeyValuePair<string, string>> detachedContentValues, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            switch (conversionType)
            {
                case DetachedValuesConversionType.Editor:
                    return ConvertDbToEditor(contentType, detachedContentValues);

                case DetachedValuesConversionType.Db:
                    return ConvertEditorToDb(contentType, detachedContentValues);

                default:
                    return ConvertEditorToDb(contentType, detachedContentValues);
            }
        }

        /// <summary>
        /// Converts the detached value collection to property values for various usages depending on type passed.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="dcv">
        /// The detached content value.
        /// </param>
        /// <param name="conversionType">
        /// The conversion type.
        /// </param>
        /// <returns>
        /// The converted values.
        /// </returns>
        public KeyValuePair<string, string> Convert(IContentType contentType, KeyValuePair<string, string> dcv, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db)
        {
            switch (conversionType)
            {
                case DetachedValuesConversionType.Editor:
                    return ConvertDbToEditor(contentType, dcv);

                case DetachedValuesConversionType.Db:
                    return ConvertEditorToDb(contentType, dcv);

                default:
                    return ConvertEditorToDb(contentType, dcv);
            }
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

            return GetContentTypeByKey(detachedContentType.ContentTypeKey.Value);
        }

        /// <summary>
        /// Gets <see cref="IContentType"/> by it's unique id.
        /// </summary>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IContentType"/>.
        /// </returns>
        internal IContentType GetContentTypeByKey(Guid contentTypeKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(contentTypeKey), "contentTypeKey");

            return _contentTypeService.GetContentType(contentTypeKey);
        }

        /// <summary>
        /// A method to deserialize the values that has been saved by an editor
        ///             to an object to be stored in the database.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="detachedContentValues">
        /// The detached content values.
        /// </param>
        /// <returns>
        /// The converted values.
        /// </returns>
        /// <remarks>
        /// By default this will attempt to automatically convert the string value to the value type supplied by ValueType.
        /// 
        ///             If overridden then the object returned must match the type supplied in the ValueType, otherwise persisting the
        ///             value to the DB will fail when it tries to validate the value type.
        /// 
        /// </remarks>
        private IEnumerable<KeyValuePair<string, string>> ConvertEditorToDb(IContentType contentType, IEnumerable<KeyValuePair<string, string>> detachedContentValues)
        {
            if (contentType == null || !_ready) return detachedContentValues;

            return detachedContentValues.ToArray().Select(dcv => this.ConvertEditorToDb(contentType, dcv)).ToList();
        }

        /// <summary>
        /// A method to deserialize the value that has been saved by an editor
        ///             to an object to be stored in the database.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="dcv">
        /// The detached content value.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        private KeyValuePair<string, string> ConvertEditorToDb(IContentType contentType, KeyValuePair<string, string> dcv)
        {
            var propType = contentType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias == dcv.Key);
            if (propType == null)
            {
                return dcv;
            }

            // Lookup the property editor
            var propEditor = PropertyEditorResolver.Current.GetByAlias(propType.PropertyEditorAlias);

            if (propEditor.ValueEditor.IsReadOnly) return dcv;

            // Fetch the property types prevalue
            var propPreValues = _dataTypeService.GetPreValuesCollectionByDataTypeId(propType.DataTypeDefinitionId);

            // Create a fake content property data object
            var contentPropData = new ContentPropertyData(dcv.Value, propPreValues, new Dictionary<string, object>());

            var rawValue = !JsonHelper.IsJsonObject(dcv.Value) ?
                    JsonConvert.DeserializeObject(dcv.Value) :
                    dcv.Value;

            try
            {
                // Get the property editor to do it's conversion

                //// TODO - this is the new place the serialization starts to error
                var newValue = propEditor.ValueEditor.ConvertEditorToDb(contentPropData, rawValue);

                // Store the value back
                var value = newValue == null ? null : JsonConvert.SerializeObject(newValue);

                return new KeyValuePair<string, string>(dcv.Key, value);
            }
            catch (Exception ex)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("ValueEditor");
                MultiLogHelper.WarnWithException<DetachedValuesConverter>(
                    "Failed to convert property value for Database",
                    ex,
                    logData);

                return dcv;
            }

        }

        /// <summary>
        /// A method used to format the database values to a value that can be used by the editor
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="detachedContentValues">
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
        private IEnumerable<KeyValuePair<string, string>> ConvertDbToEditor(IContentType contentType, IEnumerable<KeyValuePair<string, string>> detachedContentValues)
        {
            if (contentType == null || !_ready) return detachedContentValues;

            return detachedContentValues.ToArray().Select(dcv => this.ConvertDbToEditor(contentType, dcv)).ToList();
        }

        /// <summary>
        /// A method used to format the database value to a value that can be used by the editor.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="dcv">
        /// The detached content value.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        private KeyValuePair<string, string> ConvertDbToEditor(IContentType contentType, KeyValuePair<string, string> dcv)
        {
            var propType = contentType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias == dcv.Key);
            if (propType == null)
            {
                return dcv;
            }

            // Lookup the property editor
            var propEditor = PropertyEditorResolver.Current.GetByAlias(propType.PropertyEditorAlias);

            if (propEditor.ValueEditor.IsReadOnly) return dcv;

            var rawValue = !JsonHelper.IsJsonObject(dcv.Value) ? 
                JsonConvert.DeserializeObject(dcv.Value) : 
                dcv.Value;

            //// Adapted from Nested Content
            // Create a fake property using the property to add the stored value
            var prop = new Property(propType, rawValue);

            // Get the editor to do it's conversion
            try
            {
                var editorObject = propEditor.ValueEditor.ConvertDbToEditor(prop, propType, _dataTypeService);
                var json = JsonConvert.SerializeObject(editorObject);
                return new KeyValuePair<string, string>(dcv.Key, json);
            }
            catch (Exception ex)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("ValueEditor");
                MultiLogHelper.WarnWithException<DetachedValuesConverter>(
                    "Failed to convert property value for Editor",
                    ex,
                    logData);

                return dcv;
            }
        }
    }
}