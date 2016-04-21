namespace Merchello.Web.Pluggable
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
        public override bool CreateNewView(string fileName, PluginViewType viewType, string modelName, string viewBody)
        {
            var virtualPath = GetVirtualPathByPlugViewType(viewType);
            var mapped = EnsureMappedPath(virtualPath);

            var fullFileName = string.Format("{0}{1}", mapped, fileName);

            if (!File.Exists(fullFileName))
            {
                using (var writer = new StreamWriter(fullFileName))
                {
                    var heading = string.Format("@inherits UmbracoViewPage<{0}>", modelName);
                    writer.WriteLine(heading);
                    writer.WriteLine("@using Merchello.Core.Models");
                    writer.Close();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves an existing view.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <param name="viewBody">
        /// The view body.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the save was successful.
        /// </returns>
        public override bool SaveView(string fileName, PluginViewType viewType, string viewBody)
        {
            var virtualPath = GetVirtualPathByPlugViewType(viewType);
            var mapped = EnsureMappedPath(virtualPath);

            var fullFileName = string.Format("{0}{1}", mapped, fileName);
            if (File.Exists(fullFileName))
            {
                File.WriteAllText(fullFileName, viewBody);

                return true;
            }

            return false;
        }
    }
}