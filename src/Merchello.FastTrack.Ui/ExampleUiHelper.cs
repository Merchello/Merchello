namespace Merchello.FastTrack.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.Ui.Rendering;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// A utility helper class for the example store.
    /// </summary>
    /// <remarks>
    /// There are many ways to do these sorts of "site specific" common operations in Umbraco.  This class is not required
    /// for Merchello implementations.  It merely provides some short cuts for the example store implementation.
    /// </remarks>
    public static class ExampleUiHelper
    {
        /// <summary>
        /// Utilities to easily find common store content pages.
        /// </summary>
        public static class Content
        {
            #region Content Type Aliases

            /// <summary>
            /// The Umbraco ContentTypeAlias for the "store" ContentType.
            /// </summary>
            private const string ContentTypeAliasStore = "store";

            /// <summary>
            /// The Umbraco ContentTypeAlias for the "basket" ContentType.
            /// </summary>
            private const string ContentTypeAliasBasket = "basket";

            /// <summary>
            /// The Umbraco ContentTypeAlias for the "catalog" ContentType.
            /// </summary>
            private const string ContentTypeAliasCatalog = "catalog";

            /// <summary>
            /// The Umbraco ContentTypeAlias for the "checkout" ContentTypes
            /// </summary>
            private const string ContentTypeAliasCheckout = "checkout";

            /// <summary>
            /// The Umbraco ContentTypeAlias for the "receipt" ContentType.
            /// </summary>
            private const string ContentTypeAliasReceipt = "receipt";

            /// <summary>
            /// The Umbraco ContentTypeAlias for the "account" ContentType.
            /// </summary>
            private const string ContentTypeAliasAccount = "account";

            #endregion


            /// <summary>c
            /// Gets the <see cref="UmbracoContext"/>.
            /// </summary>
            private static UmbracoContext UmbracoContext
            {
                get
                {
                    if (UmbracoContext.Current == null)
                    {
                        var nullReference = new NullReferenceException("UmbracoContext was null");
                        MultiLogHelper.Error(typeof(ExampleUiHelper), "The StoreUiHelper.Content requires a current UmbracoContext", nullReference);
                        throw nullReference;
                    }

                    return UmbracoContext.Current;
                }
            }

            /// <summary>
            /// Gets the first store ContentType found.
            /// </summary>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetStoreRoot()
            {
                return UmbracoContext.ContentCache.GetByXPath(string.Format("//root/{0}", ContentTypeAliasStore)).FirstOrDefault();
            }

            /// <summary>
            /// Gets the first child of the store root content with content type alias of 'basket'.
            /// </summary>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetBasket()
            {
                return GetStoreRoot().FirstChild(x => x.ContentType.Alias == ContentTypeAliasBasket);
            }

            /// <summary>
            /// Gets the first child of the store root content with content type alias of 'catalog'.
            /// </summary>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetCatalog()
            {
                return GetStoreRoot().FirstChild(x => x.ContentType.Alias == ContentTypeAliasCatalog);
            }

            /// <summary>
            /// Gets the first child of the store root content with content type alias of 'checkout'.
            /// </summary>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetCheckout()
            {
                return GetStoreRoot().FirstChild(x => x.ContentType.Alias == ContentTypeAliasCheckout);
            }

            /// <summary>
            /// Gets the first child of the store root content with content type alias of 'receipt'.
            /// </summary>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetReceipt()
            {
                return GetStoreRoot().FirstChild(x => x.ContentType.Alias == ContentTypeAliasReceipt);
            }

            /// <summary>
            /// Gets the first child of store root content with content type alias of 'account'.
            /// </summary>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetAccount()
            {
                return GetStoreRoot().FirstChild(x => x.ContentType.Alias == ContentTypeAliasAccount);
            }

            /// <summary>
            /// Gets a category page by a collection key.
            /// </summary>
            /// <param name="collectionKey">
            /// The collection key.
            /// </param>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            public static IPublishedContent GetCategoryPageForCollection(Guid collectionKey)
            {
                var catalog = GetCatalog();

                return catalog.FirstChild(x => x.GetPropertyValue<ProductContentListView>("products").CollectionKey == collectionKey);
            }
        }

        /// <summary>
        /// Assists in mapping URLs for the example store checkout work flow.
        /// </summary>
        public static class CheckoutWorkflow
        {
            /// <summary>
            /// Gets a checkout stage <see cref="IPublishedContent"/>.
            /// </summary>
            /// <param name="stage">
            /// The stage.
            /// </param>
            /// <returns>
            /// The <see cref="IPublishedContent"/>.
            /// </returns>
            /// <remarks>
            /// The checkout stage is referenced on checkout pages as a custom property (Merchello Checkout Stage Picker)
            /// </remarks>
            public static IPublishedContent GetPageForStage(CheckoutStage stage)
            {
                var checkout = Content.GetCheckout();
                if (stage == CheckoutStage.Custom || 
                    stage == CheckoutStage.None ||
                    !checkout.Children.Any()) return checkout;

                var stagePage =
                    checkout.Children.FirstOrDefault(
                        x =>
                        x.GetPropertyValue<string>("checkoutStage")
                            .Equals(stage.ToString(), StringComparison.InvariantCultureIgnoreCase));

                return stagePage ?? checkout;
            }

            /// <summary>
            /// Gets the payment pages <see cref="IPublishedContent"/>.
            /// </summary>
            /// <returns>
            /// The <see cref="IEnumerable{IPublishedContent}"/>.
            /// </returns>
            public static IEnumerable<IPublishedContent> GetPaymentStagePages()
            {
                var checkout = Content.GetCheckout();
                return checkout.Children.Where(
                           x =>
                           x.GetPropertyValue<string>("checkoutStage")
                               .Equals("Payment", StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}