namespace Merchello.Web.Models
{
    using Merchello.Core.Models;

    /// <summary>
    /// Defines an entity collection proxy.
    /// </summary>
    public interface IEntityCollectionProxy : IHasParent, IEntityProxy, IHasExtendedData
    {
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