namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    /// <summary>
    /// Defines a product filter.
    /// </summary>
    public interface IProductFilter
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Gets the provider key.
        /// </summary>
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
    }
}