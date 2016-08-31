namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    /// <summary>
    /// Defines meta information about the provider that manages a collection or filter.
    /// </summary>
    public interface IProviderMeta
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a value indicating whether manages unique collection.
        /// </summary>
        bool ManagesUniqueCollection { get; }
    }
}