namespace Merchello.Web.Models.ContentEditing.Content
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.ValueConverters;

    using Umbraco.Core.Models;
    using Umbraco.Web.Models.ContentEditing;

    /// <summary>
    /// Assists with detached content value saves.
    /// </summary>
    internal class DetachedContentHelper
    {
        /// <summary>
        /// Gets the updated detached data values.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="detachedContentItem">
        /// The detached content item.
        /// </param>
        /// <typeparam name="TSaveModel">
        /// The type of the save model
        /// </typeparam>
        /// <typeparam name="TDisplay">
        /// The type of the display object
        /// </typeparam>
        /// <returns>
        /// The list of updated values.
        /// </returns>
        public static List<KeyValuePair<string, string>> GetUpdatedValues<TSaveModel, TDisplay>(IContentType contentType, TSaveModel detachedContentItem)
            where TSaveModel : DetachedContentSaveItem<TDisplay>
            where TDisplay : IHaveDetachedDataValues
        {
            // a new container for property values returning from editors
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

                var dcv = detachedContentItem.Display.DetachedDataValues.FirstOrDefault(x => x.Key == p.Alias);

                // only convert and add the property if it still exists on the content type
                if (DetachedValuesConverter.Current.VerifyPropertyExists(contentType, dcv.Key))
                {
                    updatedValues.Add(DetachedValuesConverter.Current.Convert(contentType, dcv, additionalData: d));
                }
            }

            return updatedValues;
        }
    }
}