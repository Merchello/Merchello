namespace Merchello.Bazaar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;

    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The content helper.
    /// </summary>
    public static class BazaarContentHelper
    {
        /// <summary>
        /// The content helper cached content type alias.
        /// </summary>
        private static readonly string[] ContentHelperCachedContentTypeAlias =
            {
                "BazaarAccount",
                "BazaarAccountHistory",
                "BazaarBasket",
                "BazaarCheckout",
                "BazaarCheckoutConfirm",
                "BazaarCheckoutShipping",                
                "BazaarProduct",
                "BazaarProductCollection",
                "BazaarProductContent",
                "BazaarProductGroup",
                "BazaarReciept",
                "BazaarRegistration",
                "BazaarStore",
                "BazaarWishList"
            };

        /// <summary>
        /// Gets or sets the store root.
        /// </summary>
        internal static IPublishedContent StoreRoot { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        internal static string Theme { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        internal static ICurrency Currency { get; set; }

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
        /// The get store theme.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetStoreTheme()
        {
            if (!Theme.IsNullOrWhiteSpace()) return Theme;
            Theme = GetStoreRoot().GetPropertyValue<string>("themePicker");
            return Theme;
        }

        /// <summary>
        /// Gets the store currency.
        /// </summary>
        /// <returns>
        /// The <see cref="ICurrency"/>.
        /// </returns>
        public static ICurrency GetStoreCurrency()
        {
            if (Currency != null) return Currency;
            var storeSettingsService = MerchelloContext.Current.Services.StoreSettingService;
            var storeSetting = storeSettingsService.GetByKey(Core.Constants.StoreSettingKeys.CurrencyCodeKey);
            Currency = storeSettingsService.GetCurrencyByCode(storeSetting.Value);
            return Currency;
        }

        /// <summary>
        /// Gets the store title.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string StoreTitle()
        {
            return GetStoreRoot().GetPropertyValue<string>("storeTitle");
        }

        /// <summary>
        /// Gets a value indicating whether or not the wish list should be shown.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ShowWishList()
        {
            return GetStoreRoot().GetPropertyValue<bool>("enableWishList");
        }

        /// <summary>
        /// Gets a value indicating whether or not to show the customer account.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ShowAccount()
        {
            return GetStoreRoot().GetPropertyValue<bool>("customerAccounts");
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
        /// Gets the product groups content.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedContent}"/>.
        /// </returns>
        [Obsolete("Start using product collections")]
        public static IEnumerable<IPublishedContent> GetProductGroupsContent()
        {
            return GetStoreRoot().Children.Where(x => x.DocumentTypeAlias == "BazaarProductGroup" && x.IsVisible());
        }

        /// <summary>
        /// Gets the product collection content.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedContent}"/>.
        /// </returns>
        public static IEnumerable<IPublishedContent> GetProductCollectionContent()
        {
            return GetStoreRoot().Children.Where(x => x.DocumentTypeAlias == "BazaarProductCollection" && x.IsVisible());
        }

        /// <summary>
        /// The reset.
        /// </summary>
        /// <param name="contentTypes">
        /// The content types.
        /// </param>
        internal static void Reset(IEnumerable<IContentType> contentTypes)
        {
            contentTypes.ForEach(Reset);
        }

        /// <summary>
        /// The reset.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        internal static void Reset(IContentType contentType)
        {
            if (ContentHelperCachedContentTypeAlias.Contains(contentType.Alias))
            {
                StoreRoot = null;
                Theme = null;
                Currency = null;
            }
        }
    }
}