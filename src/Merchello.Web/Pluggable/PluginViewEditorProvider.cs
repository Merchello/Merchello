namespace Merchello.Web.Pluggable
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Web.Models.ContentEditing.Templates;

    using Umbraco.Core;

    /// <summary>
    /// A service for working with Merchello Plugin generated views.
    /// </summary>
    internal class PluginViewEditorProvider : PluginViewProviderBase<PluginViewEditorContent>
    {
        /// <summary>
        /// Gets all views at or below the virtual path.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TModel}"/>.
        /// </returns>
        public override IEnumerable<PluginViewEditorContent> GetAllViews(string virtualPath)
        {
            if (virtualPath.IsNullOrWhiteSpace()) return Enumerable.Empty<PluginViewEditorContent>();

            var path = EnsureMappedPath(virtualPath);

            var dir = new DirectoryInfo(path);

            var files = dir.GetFiles("*.cshtml", SearchOption.AllDirectories).OrderBy(x => x.Name);
            return files.Select(x => x.ToAppPluginViewEditorContent(virtualPath)).Where(x => x != null);
        }

        /// <summary>
        /// Gets a specific view.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <returns>
        /// The <see cref="PluginViewEditorContent"/>.
        /// </returns>
        public override PluginViewEditorContent GetView(string virtualPath, string fileName, PluginViewType viewType)
        {
            if (virtualPath.IsNullOrWhiteSpace()) throw new System.Exception("VirtualPath cannot be null or whitespace");

            var mapped = EnsureMappedPath(virtualPath);

            var fullFileName = string.Format("{0}{1}", mapped, fileName.Replace(" ", string.Empty));

            if (!File.Exists(fullFileName))
            {
                var nullRef = new NullReferenceException("File does not exist on disk.");
                var logData = MultiLogger.GetBaseLoggingData();
                MultiLogHelper.Error<PluginViewEditorProvider>("File does not exist.", nullRef, logData);
                throw nullRef;
            }

            var file = new FileInfo(fullFileName);

            return file.ToAppPluginViewEditorContent(virtualPath);
        }

        /// <summary>
        /// Creates a new view.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <param name="modelName">
        /// The model name.
        /// </param>
        /// <param name="viewBody">
        /// The view body.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the create was successful.
        /// </returns>
        public override PluginViewEditorContent CreateNewView(string fileName, PluginViewType viewType, string modelName, string viewBody)
        {
            var virtualPath = GetVirtualPathByPlugViewType(viewType);

            var mapped = EnsureMappedPath(virtualPath);

            fileName = fileName.Replace(" ", string.Empty);

            var fullFileName = string.Format("{0}{1}", mapped, fileName);

            if (!File.Exists(fullFileName))
            {
                using (var sw = File.CreateText(fullFileName))
                {
                    sw.WriteLine("@inherits Merchello.Web.Mvc.MerchelloHelperViewPage<{0}>", modelName);
                    sw.WriteLine("@using Merchello.Core");
                    sw.WriteLine("@using Merchello.Core.Models");
                    sw.WriteLine("@*");
                    sw.WriteLine("     MerchelloHelperViewPage<T> inherits from UmbracoViewPage<t> and exposes the MerchelloHelper as 'Merchello'");
                    sw.WriteLine("     Example usage:  var product = Merchello.TypedProductContent(YOURPRODUCTKEY);");
                    sw.WriteLine("*@");
                    sw.Close();
                }
                    
                return GetView(virtualPath, fileName, viewType);
            }

            var logData = MultiLogger.GetBaseLoggingData();
            var ex = new InvalidDataException("File already exists");
            MultiLogHelper.Error<PluginViewEditorProvider>("Cannot create a duplicate file", ex, logData);
            throw ex;
        }
    }
}