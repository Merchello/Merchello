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
        
            virtualPath = EnsureMappedPath(virtualPath);

            var dir = new DirectoryInfo(virtualPath);

            var files = dir.GetFiles("*.cshtml", SearchOption.AllDirectories).OrderBy(x => x.Name);
            return files.Select(x => x.ToAppPluginViewEditorContent()).Where(x => x != null);
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
    }
}