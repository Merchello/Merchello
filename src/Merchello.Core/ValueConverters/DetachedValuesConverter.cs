namespace Merchello.Core.ValueConverters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.ValueConverters.ValueCorrections;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Editors;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;

    /// <summary>
    /// A converter to assist in saving detached property data correctly.
    /// </summary>
    public class DetachedValuesConverter
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
        /// The <see cref="MediaService"/>.
        /// </summary>
        private readonly IMediaService _mediaService;

        /// <summary>
        /// Internal class for correcting stored detached values.
        /// </summary>
        private readonly DetachedValueCorrector _corrector;

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
        /// <param name="values">
        /// The resolved DefaultValueCorrection types.
        /// </param>
        internal DetachedValuesConverter(ApplicationContext applicationContext, IEnumerable<Type> values)
        {
            if (applicationContext != null)
            {
                _contentTypeService = applicationContext.Services.ContentTypeService;
                _dataTypeService = applicationContext.Services.DataTypeService;
                _mediaService = applicationContext.Services.MediaService;
                _ready = true;
            }
            else
            {
                _ready = false;
            }

            // Instantiate the corrector
            _corrector = new DetachedValueCorrector(values);
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
        /// Gets the current.
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
        /// Verifies a property still exists on the content type.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the property exists.
        /// </returns>
        /// <remarks>
        /// In cases where the property has been removed, we don't want to store previously saved values.
        /// </remarks>
        public bool VerifyPropertyExists(IContentType contentType, string propertyAlias)
        {
            return contentType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias == propertyAlias) != null;
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
        /// <param name="additionalData">
        /// A dictionary of additional data (only used with DetachedValuesConversionType database).
        /// </param>
        /// <returns>
        /// The converted values.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> Convert(IContentType contentType, IEnumerable<KeyValuePair<string, string>> detachedContentValues, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db, Dictionary<string, object> additionalData = null)
        {
            switch (conversionType)
            {
                case DetachedValuesConversionType.Editor:
                    return ConvertDbToEditor(contentType, detachedContentValues);

                case DetachedValuesConversionType.Db:
                    return ConvertEditorToDb(contentType, detachedContentValues, additionalData);

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
        /// <param name="additionalData">
        /// A dictionary of additional data (only used with DetachedValuesConversionType database).
        /// </param>
        /// <returns>
        /// The converted values.
        /// </returns>
        public KeyValuePair<string, string> Convert(IContentType contentType, KeyValuePair<string, string> dcv, DetachedValuesConversionType conversionType = DetachedValuesConversionType.Db, Dictionary<string, object> additionalData = null)
        {
            switch (conversionType)
            {

                case DetachedValuesConversionType.Editor:
                    return ConvertDbToEditor(contentType, dcv);

                case DetachedValuesConversionType.Db:
                    return ConvertEditorToDb(contentType, dcv, additionalData);

                default:
                    return ConvertEditorToDb(contentType, dcv, additionalData);
            }
        }

        /// <summary>
        /// The converts the stored value for content.
        /// </summary>
        /// <param name="publishedPropertyType">
        /// The published property type.
        /// </param>
        /// <param name="dcv">
        /// The detached content value.
        /// </param>
        /// <returns>
        /// The value for displaying in <see cref="IPublishedContent"/>.
        /// </returns>
        public KeyValuePair<string, object> ConvertDbForContent(PublishedPropertyType publishedPropertyType, KeyValuePair<string, string> dcv)
        {
            var value = _corrector.CorrectedValue(publishedPropertyType.PropertyEditorAlias, JsonConvert.DeserializeObject(dcv.Value));
            return new KeyValuePair<string, object>(dcv.Key, value);
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
        /// <param name="additionalData">
        /// A dictionary of additional data ex. file uploads (only used with DetachedValuesConversionType database).
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
        private IEnumerable<KeyValuePair<string, string>> ConvertEditorToDb(IContentType contentType, IEnumerable<KeyValuePair<string, string>> detachedContentValues, IDictionary<string, object> additionalData = null)
        {
            if (contentType == null || !_ready) return detachedContentValues;

            return detachedContentValues.ToArray().Select(dcv => this.ConvertEditorToDb(contentType, dcv, additionalData)).ToList();
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
        /// <param name="additionalData">
        /// The additional Data.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        private KeyValuePair<string, string> ConvertEditorToDb(IContentType contentType, KeyValuePair<string, string> dcv, IDictionary<string, object> additionalData)
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

            var rawValue = JsonConvert.DeserializeObject(dcv.Value.Trim());

            //// Create a fake content property data object
            if (additionalData == null) additionalData = new Dictionary<string, object>();

            //The image cropper wants the content key and property key to save correctly.
            if (propEditor.Alias == "Umbraco.ImageCropper")
            {
                additionalData.Add("cuid", contentType.Key);
                additionalData.Add("puid", propType.Key);
            }

            var contentPropData = new ContentPropertyData(rawValue, propPreValues, additionalData);

            try
            {
                // Get the property editor to do it's conversion
                var newValue = propEditor.ValueEditor.ConvertEditorToDb(contentPropData, null);

                // Store the value back
                var value = newValue == null ? string.Empty :
                    JsonHelper.IsJsonObject(newValue) ?
                                    newValue.ToString() :
                                    JsonConvert.SerializeObject(newValue);


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

            object rawValue;
            try
            {
                rawValue = !JsonHelper.IsJsonObject(dcv.Value) ?
                    JsonConvert.DeserializeObject(dcv.Value) :
                    dcv.Value;
            }
            catch (Exception ex)
            {
                // try to correct the value 
                rawValue = TryFixLegacyValue(dcv.Value);
            }


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

        /// <summary>
        /// The try fix legacy value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object TryFixLegacyValue(string value)
        {
            // Assume this is a value was saved as a string and not 
            // sent to the db converter.
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value;
        }

        /// <summary>
        /// Allows for overriding stored detached values with corrections.
        /// </summary>
        /// <remarks>
        /// Generally used for legacy data types or handling problems with the way Merchello stores property data during JSON serialization
        /// </remarks>
        protected class DetachedValueCorrector
        {
            /// <summary>
            /// The cache of corrections.
            /// </summary>
            private readonly ConcurrentDictionary<string, IDetachedValueCorrection> _correctionCache = new ConcurrentDictionary<string, IDetachedValueCorrection>();

            /// <summary>
            /// Initializes a new instance of the <see cref="DetachedValueCorrector"/> class.
            /// </summary>
            /// <param name="values">
            /// The values.
            /// </param>
            public DetachedValueCorrector(IEnumerable<Type> values)
            {
                BuildCache(values);
            }

            /// <summary>
            /// Applies the resolved correction if there are any.
            /// </summary>
            /// <param name="propertyEditorAlias">
            /// The property editor alias.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <returns>
            /// The corrected value <see cref="object"/>.
            /// </returns>
            public object CorrectedValue(string propertyEditorAlias, object value)
            {
                var correction = this._correctionCache.FirstOrDefault(x => x.Key == propertyEditorAlias).Value;
                return correction == null ? value : correction.ApplyCorrection(value);
            }

            /// <summary>
            /// Builds the type cache.
            /// </summary>
            /// <param name="values">
            /// The values.
            /// </param>
            private void BuildCache(IEnumerable<Type> values)
            {
                foreach (var attempt in values.Select(type => ActivatorHelper.CreateInstance<DetachedValueCorrectionBase>(type, new object[] { })).Where(attempt => attempt.Success))
                {
                    this.AddOrUpdateCache(attempt.Result);
                }
            }

            /// <summary>
            /// Adds or updates a <see cref="IDetachedValueCorrection"/> instance to the concurrent cache.
            /// </summary>
            /// <param name="correction">
            /// The correction.
            /// </param>
            private void AddOrUpdateCache(IDetachedValueCorrection correction)
            {
                var att = correction.GetType().GetCustomAttribute<DetachedValueCorrectionAttribute>(false);
                if (att != null)
                {
                    this._correctionCache.AddOrUpdate(att.PropertyEditorAlias, correction, (x, y) => correction);
                }
            }
        }
    }
}