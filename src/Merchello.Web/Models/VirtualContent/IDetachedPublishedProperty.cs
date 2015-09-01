namespace Merchello.Web.Models.DetachedContent
{
    using Umbraco.Core.Models;

    /// <summary>
    /// Defines a detached published property.
    /// </summary>
    public interface IDetachedPublishedProperty : IPublishedProperty
    {
        /// <summary>
        /// Gets the culture name.
        /// </summary>
        string CultureName { get; }
    }
}