namespace Merchello.Web.Models.ContentEditing.Content
{
    using System.Collections.Generic;

    using Umbraco.Web.Models.ContentEditing;

    /// <summary>
    /// Saves a display item's detached content.
    /// </summary>
    /// <typeparam name="TDisplay">
    /// The type of display object
    /// </typeparam>
    public abstract class DetachedContentSaveItem<TDisplay> : IHaveUploadedFiles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentSaveItem{TDisplay}"/> class.
        /// </summary>
        protected DetachedContentSaveItem()
        {
            this.UploadedFiles = new List<ContentItemFile>();
        }

        /// <summary>
        /// Gets or sets the detached content item.
        /// </summary>
        public TDisplay Display { get; set; }


        /// <summary>
        /// Gets the uploaded files.
        /// </summary>
        public List<ContentItemFile> UploadedFiles { get; private set; }
    }
}