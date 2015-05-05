namespace Merchello.Core.Marketing
{
    using System;

    /// <summary>
    /// An attribute to decorate a campaign activity class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CampaignActivityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivityAttribute"/> class.
        /// </summary>
        /// <param name="typeKey">
        /// The type key.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="editorView">
        /// The editor view.
        /// </param>
        public CampaignActivityAttribute(string typeKey, string title, string description, string editorView)
        {
            TypeKey = new Guid(typeKey);
            Title = title;
            Description = description;
            EditorView = editorView;
        }

        /// <summary>
        /// Gets the type key.
        /// </summary>
        public Guid TypeKey { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the editor view.
        /// </summary>
        public string EditorView { get; private set; }
    }
}