namespace Merchello.Core.Configuration.BackOffice
{
    using System.Collections.Generic;

    using Merchello.Core.EntityCollections;

    /// <summary>
    /// Represents a dashboard node in back office (administrative) tree.
    /// </summary>
    /// REVIEW - naming
    public interface IDashboardTreeNode : IDashboardTreeNodeMeta
    {
        /// <summary>
        /// Gets a value indicating whether self managed providers before static providers.
        /// </summary>
        bool SelfManagedProvidersBeforeStaticProviders { get; }

        /// <summary>
        /// Gets the collection of self managed entity collection providers.
        /// </summary>
        /// <remarks>
        /// Self managed providers are simply providers for collections that do not have user assigned items.
        /// Example: Unpaid invoices.
        /// </remarks>
        IEnumerable<IDashboardTreeNodeKeyLink> SelfManagedEntityCollectionProviders { get; }
    }
}