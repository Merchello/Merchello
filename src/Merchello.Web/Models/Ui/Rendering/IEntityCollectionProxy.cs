namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    /// <summary>
    /// Defines an entity collection proxy.
    /// </summary>
    public interface IEntityCollectionProxy : IEntityProxy
    {
        /// <summary>
        /// Gets the parent key.
        /// </summary>
        Guid? ParentKey { get; }

        /// <summary>
        /// Gets the collection name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Gets information about the managing provider.
        /// </summary>
        IProviderMeta ProviderMeta { get; }
    }
}