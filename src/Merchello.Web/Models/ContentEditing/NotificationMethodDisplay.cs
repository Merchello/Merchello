namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The notification method display.
    /// </summary>
    public class NotificationMethodDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Gets or sets the notification messages.
        /// </summary>
        public IEnumerable<NotificationMessageDisplay> NotificationMessages { get; set; } 
    }
}