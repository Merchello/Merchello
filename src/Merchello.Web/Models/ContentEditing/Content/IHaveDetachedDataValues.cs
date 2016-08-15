namespace Merchello.Web.Models.ContentEditing.Content
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an object that has detached data values.
    /// </summary>
    public interface IHaveDetachedDataValues
    {
        /// <summary>
        /// Gets or sets the detached data values.
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> DetachedDataValues { get; set; }
    }
}