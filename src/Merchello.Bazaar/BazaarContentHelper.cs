namespace Merchello.Bazaar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;

    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The content helper.
    /// </summary>
    public static class BazaarContentHelper
    {
        /// <summary>
        /// Gets or sets the store root.
        /// </summary>
        internal static IPublishedContent StoreRoot { get; set; }

        /// <summary>
        /// The get store root.
        /// </summary>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        public static IPublishedContent GetStoreRoot()
        {
            if (StoreRoot != null) return StoreRoot;
            var umbraco = new UmbracoHelper(UmbracoContext.Current);
            StoreRoot = umbraco.TypedContentSingleAtXPath(WebConfigurationManager.AppSettings["Bazaar:XpathToStore"]);
            return StoreRoot;
        }
     
        /// <summary>
        /// Gets the basket content.
        /// </summary>
        /// <returns>
        /// The <see cref="BasketModel"/>.
        /// </returns>
        public static IPublishedContent GetBasketContent()
        {
            return GetStoreRoot().Children.FirstOrDefault(x => x.DocumentTypeAlias == "BazaarBasket");
        }

        /// <summary>
        /// Gets the registration content.
        /// </summary>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        public static IPublishedContent GetRegistrationContent()
        {
            return GetStoreRoot().Descendant("BazaarRegistration");
        }

        /// <summary>
        /// Gets the account content.
        /// </summary>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        public static IPublishedContent GetAccountContent()
        {
            return GetStoreRoot().Descendant("BazaarAccount");
        }

        /// <summary>
        /// Gets wish list content.
        /// </summary>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        public static IPublishedContent GetWishListContent()
        {
            return GetStoreRoot().Descendant("BazaarWishList");
        }

        /// <summary>
        /// Gets the checkout page content.
        /// </summary>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        public static IPublishedContent GetCheckoutPageContent()
        {
            return GetStoreRoot().Descendant("BazaarCheckout");
        }

        /// <summary>
        /// The get continue shopping content.
        /// </summary>
        /// <returns>
        /// The <see cref="IPublishedContent"/>.
        /// </returns>
        [Obsolete("Start using product collections")]
        public static IPublishedContent GetContinueShoppingContent()
        {
            return GetProductGroupsContent().Any() ? GetProductGroupsContent().First() : GetStoreRoot();
        }

        /// <summary>
        /// The product groups content.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedContent}"/>.
        /// </returns>
        [Obsolete("Start using product collections")]
        public static IEnumerable<IPublishedContent> GetProductGroupsContent()
        {
            return GetStoreRoot().Children.Where(x => x.DocumentTypeAlias == "BazaarProductGroup" && x.IsVisible());
        }
    }
}