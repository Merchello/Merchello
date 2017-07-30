namespace Merchello.Core.Models
{
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a store.
    /// </summary>
    public interface IStore : IEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// Gets the <see cref="StoreSettingsCollection"/>.
        /// </summary>
        //StoreSettingsCollection Settings { get; }
    }
}