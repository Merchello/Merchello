namespace Merchello.Web.IO
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Web.Models.ContentEditing.Templates;

    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// A helper for manipulating Merchello Plugin generated views.
    /// </summary>
    internal static class PluginViewHelper
    {
        /// <summary>
        /// Gets all the ".
        /// </summary>
        /// <param name="virtualPath">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{AppPluginViewEditorContent}"/>.
        /// </returns>
        public static IEnumerable<PluginViewEditorContent> GetAllViews(string virtualPath)
        {
            if (virtualPath.IsNullOrWhiteSpace()) return Enumerable.Empty<PluginViewEditorContent>();
        
            var path = EnsureMappedPath(virtualPath);

            var dir = new DirectoryInfo(path);

            var files = dir.GetFiles("*.cshtml", SearchOption.AllDirectories).OrderBy(x => x.Name);
            return files.Select(x => x.ToAppPluginViewEditorContent(virtualPath)).Where(x => x != null);
        }

        /// <summary>
        /// Creates a new view file.
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
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool CreateNewView(string fileName, PluginViewType viewType, string modelName, string viewBody)
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
        /// Overwrites a view.
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
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool SaveView(string fileName, PluginViewType viewType, string viewBody)
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

        /// <summary>
        /// Ensures that the mapped path exists.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EnsureMappedPath(string virtualPath)
        {
            var mapped = IOHelper.MapPath(virtualPath);
            if (!Directory.Exists(mapped))
            {
                Directory.CreateDirectory(mapped);
            }

            return mapped;
        }

        private static string GetVirtualPathByPlugViewType(PluginViewType viewType)
        {
            switch (viewType)
            {
                default:
                    return MerchelloConfiguration.Current.GetSetting("NotificationTemplateBasePath");
            }
        }
    }
}