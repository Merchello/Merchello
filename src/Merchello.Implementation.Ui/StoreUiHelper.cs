namespace Merchello.Implementation.Ui
{
    using System;
    using System.Linq;

    using Merchello.Core.Logging;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// A utility helper class for the example store.
    /// </summary>
    /// <remarks>
    /// There are many ways to do these sorts of "site specific" common operations in Umbraco.  This class is not required
    /// for Merchello implementations.  It merely provides some short cuts for the example store implementation.
    /// </remarks>
    public static class StoreUiHelper
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

            #endregion


            /// <summary>
            /// Gets the <see cref="UmbracoContext"/>.
            /// </summary>
            private static UmbracoContext UmbracoContext
            {
                get
                {
                    if (UmbracoContext.Current == null)
                    {
                        var nullReference = new NullReferenceException("UmbracoContext was null");
                        MultiLogHelper.Error(typeof(StoreUiHelper), "The StoreUiHelper.Content requires a current UmbracoContext", nullReference);
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
        }
    }
}