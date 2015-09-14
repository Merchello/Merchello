namespace Merchello.Bazaar.Models
{
    using System;

    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Core.Models;

    /// <summary>
    /// Bazaar extension methods for <see cref="IPublishedContent"/>.
    /// </summary>
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// Gets the store model.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="StoreModel"/>.
        /// </returns>
        public static StoreModel StoreModel(this IPublishedContent content)
        {
            throw new NotImplementedException();
            //return BazaarContentHelper.GetStoreModel();
        }

    }
}