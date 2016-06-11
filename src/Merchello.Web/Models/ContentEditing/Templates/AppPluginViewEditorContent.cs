namespace Merchello.Web.Models.ContentEditing.Templates
{
    using System.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The template display.
    /// </summary>
    public class PluginViewEditorContent
    {
        /// <summary>
        /// Gets or sets the virtual path.
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        public string ViewBody { get; set; }

        /// <summary>
        /// Gets or sets the plugin view type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PluginViewType PluginViewType { get; set; }

        /// <summary>
        /// Gets or sets the model type name.
        /// </summary>
        public string ModelTypeName { get; set; }
    }

    /// <summary>
    /// Extension methods for AppPluginViewEditorContent.
    /// </summary>
    internal static class PluginViewEditorContentExtensions
    {

        /// <summary>
        /// Maps <see cref="FileInfo"/> to <see cref="PluginViewEditorContent"/>.
        /// </summary>
        /// <param name="fi">
        /// The fi.
        /// </param>
        /// <param name="virtualPath">
        /// The virtual Path.
        /// </param>
        /// <param name="viewType">
        /// The view Type.
        /// </param>
        /// <returns>
        /// The <see cref="PluginViewEditorContent"/>.
        /// </returns>
        public static PluginViewEditorContent ToAppPluginViewEditorContent(this FileInfo fi, string virtualPath, PluginViewType viewType = PluginViewType.Notification)
        {
            if (fi.Extension != ".cshtml") return null;
            return new PluginViewEditorContent
                       {
                           VirtualPath = virtualPath,
                           FileName = fi.Name,
                           ViewBody = File.ReadAllText(fi.FullName),
                           PluginViewType = viewType
                       };
        }
    }
}