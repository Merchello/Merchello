namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// Base class for display objects that contain dialog editor views
    /// </summary>
    public abstract class DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the dialog editor view.
        /// </summary>
        public DialogEditorViewDisplay DialogEditorView { get; set; }
    }
}