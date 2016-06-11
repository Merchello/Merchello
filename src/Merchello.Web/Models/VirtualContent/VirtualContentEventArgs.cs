namespace Merchello.Web.Models.VirtualContent
{
    using System;

    using Umbraco.Core.Models;

    /// <summary>
    /// The virtual content event args.
    /// </summary>
    public class VirtualContentEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualContentEventArgs"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public VirtualContentEventArgs(IPublishedContent parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public IPublishedContent Parent { get; set; }
    }
}