namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    /// <summary>
    /// Defines an entity proxy.
    /// </summary>
    public interface IEntityProxy
    {
        /// <summary>
        /// Gets the collection key.
        /// </summary>
        Guid Key { get; }

    }
}