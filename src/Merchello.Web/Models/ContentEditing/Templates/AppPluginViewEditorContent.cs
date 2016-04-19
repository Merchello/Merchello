namespace Merchello.Web.Models.ContentEditing.Templates
{
    using System.IO;

    /// <summary>
    /// The template display.
    /// </summary>
    public class PluginViewEditorContent
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        public string ViewBody { get; set; }
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
        /// <returns>
        /// The <see cref="PluginViewEditorContent"/>.
        /// </returns>
        public static PluginViewEditorContent ToAppPluginViewEditorContent(this FileInfo fi)
        {
            if (fi.Extension != ".cshtml") return null;
            return new PluginViewEditorContent
                       {
                           Path = fi.FullName.Replace(fi.Name, string.Empty),
                           FileName = fi.Name,
                           ViewBody = File.ReadAllText(fi.FullName)
                       };
        }
    }
}