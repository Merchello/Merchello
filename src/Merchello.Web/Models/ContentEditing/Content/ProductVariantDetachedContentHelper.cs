namespace Merchello.Web.Models.ContentEditing.Content
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Editors;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;

    /// <summary>
    /// The product variant detached content helper.
    /// </summary>
    /// <typeparam name="TSaveModel">
    /// The Type of save model
    /// </typeparam>
    /// <typeparam name="TDisplay">
    /// The type of display object
    /// </typeparam>
    internal sealed class ProductVariantDetachedContentHelper<TSaveModel, TDisplay>
        where TSaveModel : DetachedContentSaveItem<TDisplay> where TDisplay : ProductDisplayBase
    {
        /// <summary>
        /// Maps the detached property values
        /// </summary>
        /// <param name="detachedContentItem">
        /// The detached content item.
        /// </param>
        public static void MapDetachedProperties(TSaveModel detachedContentItem)
        {
            if (!detachedContentItem.Display.DetachedContents.Any()) return;
            var updatedContent =
                detachedContentItem.Display.DetachedContents.FirstOrDefault(
                    x => x.CultureName == detachedContentItem.CultureName);
            
            if (updatedContent == null)
            {
                LogHelper.Debug(
                    typeof(ProductVariantDetachedContentHelper<TSaveModel, TDisplay>),
                    "Updated detached content was not found");
                return;
            }

            var contentTypeAlias =
                detachedContentItem.Display.DetachedContents.First().DetachedContentType.UmbContentType.Alias;

            var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            var contentType = contentTypeService.GetContentType(contentTypeAlias);

            if (contentType == null)
            {
                LogHelper.Debug(typeof(ProductVariantDetachedContentHelper<TSaveModel, TDisplay>), "ContentType could not be found");
                return;
            }

            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            var updatedValues = new List<KeyValuePair<string, string>>();
            foreach (var p in contentType.CompositionPropertyTypes.ToArray())
            {
                //// create the property data to send to the property editor

                var d = new Dictionary<string, object>();
                //// add the files if any
                var files = detachedContentItem.UploadedFiles.Where(x => x.PropertyAlias == p.Alias).ToArray();
                if (files.Any())
                {
                    d.Add("files", files);
                }
                
                var detachedValue = updatedContent.DetachedDataValues.FirstOrDefault(x => x.Key == p.Alias).Value;
                if (!detachedValue.IsNullOrWhiteSpace())
                {
                                     
                    var editor = PropertyEditorResolver.Current.GetByAlias(p.PropertyEditorAlias);
                    if (editor == null)
                    {
                        LogHelper.Warn<ProductVariantDetachedContentHelper<TSaveModel, TDisplay>>(
                            "No property editor found for property " + p.Alias);
                    }
                    else
                    {
                        // TODO there has to be a better way of doing this
                        var preValues = dataTypeService.GetPreValuesCollectionByDataTypeId(p.DataTypeDefinitionId);

                        var data = new ContentPropertyData(JsonConvert.DeserializeObject(detachedValue), preValues, d);

                        var valueEditor = editor.ValueEditor;
                        if (valueEditor.IsReadOnly == false)
                        {
                            var current = JsonConvert.DeserializeObject(detachedValue);
                            var propVal = editor.ValueEditor.ConvertEditorToDb(data, null);

                            // TODO fighting internals
                            // var supportTagsAttribute = TagExtractor.GetAttribute(p.PropertyEditor);

                            // TODO - this is a total hack around catching a string value compared to json
                            detachedValue = propVal == null ? string.Empty : 
                                propVal.ToString().StartsWith("{") && propVal.ToString().EndsWith("}") ? 
                                    propVal.ToString() : 
                                    string.Format("\"{0}\"", propVal);

                            updatedValues.Add(new KeyValuePair<string, string>(p.Alias, detachedValue));                            
                        }    
                    }                  
                }
            }

            updatedContent.DetachedDataValues = updatedValues;           
        }
    }
}